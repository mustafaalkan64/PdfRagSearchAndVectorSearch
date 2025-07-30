using PdfSemanticSearchDemo.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace PdfSemanticSearchDemo.Services;

public class RagService : IRagService
{
    private readonly IVectorService _vectorService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<RagService> _logger;
    private readonly string _ollamaBaseUrl;

    public RagService(IVectorService vectorService, HttpClient httpClient, ILogger<RagService> logger, IConfiguration configuration)
    {
        _vectorService = vectorService;
        _httpClient = httpClient;
        _logger = logger;
        _ollamaBaseUrl = configuration.GetValue<string>("Ollama:BaseUrl") ?? "http://localhost:11434";
    }

    public async Task<RagSearchResponse> SearchWithRagAsync(RagSearchRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new RagSearchResponse
        {
            Query = request.Query
        };

        try
        {
            _logger.LogInformation("Starting RAG search for query: {Query}", request.Query);

            // Step 1: Retrieve relevant documents using vector search
            var searchResults = await _vectorService.SearchAsync(
                request.Query, 
                request.MaxResults, 
                request.Threshold
            );

            if (searchResults.Count == 0)
            {
                response.GeneratedAnswer = "I couldn't find any relevant information in the uploaded documents to answer your question. Please try rephrasing your query or upload relevant documents.";
                response.Success = true;
                return response;
            }

            _logger.LogInformation("Retrieved {Count} relevant documents", searchResults.Count);

            // Step 2: Generate answer using the retrieved context
            var generatedAnswer = await GenerateAnswerAsync(request.Query, searchResults);

            // Step 3: Prepare response
            response.GeneratedAnswer = generatedAnswer;
            response.SourceDocuments = request.IncludeSourceDocuments ? searchResults : new List<SearchResult>();
            response.Success = true;

            stopwatch.Stop();
            response.ResponseTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            _logger.LogInformation("RAG search completed in {ResponseTime}ms", response.ResponseTimeMs);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RAG search for query: {Query}", request.Query);
            
            stopwatch.Stop();
            response.Success = false;
            response.ErrorMessage = "An error occurred while processing your request. Please try again.";
            response.ResponseTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            
            return response;
        }
    }

    public async Task<string> GenerateAnswerAsync(string query, List<SearchResult> context)
    {
        try
        {
            // Build context from retrieved documents
            var contextBuilder = new StringBuilder();
            contextBuilder.AppendLine("Based on the following document excerpts:");
            contextBuilder.AppendLine();

            for (int i = 0; i < context.Count; i++)
            {
                var doc = context[i];
                contextBuilder.AppendLine($"Document {i + 1} (from {doc.FileName}, page {doc.PageNumber}):");
                contextBuilder.AppendLine(doc.Content);
                contextBuilder.AppendLine();
            }

            // Create the system prompt for RAG
            var systemPrompt = @"You are an intelligent document assistant. Your task is to answer questions based on the provided document excerpts. 

Guidelines:
1. Answer the question using ONLY the information provided in the document excerpts
2. If the information is not sufficient to answer the question, say so clearly
3. Cite which document(s) and page(s) you're referencing in your answer
4. Be concise but comprehensive
5. If there are conflicting information in different documents, mention this
6. Use a professional and helpful tone

Remember: Only use the information from the provided documents. Do not add external knowledge.";

            var userPrompt = $"{contextBuilder}\n\nQuestion: {query}\n\nPlease provide a comprehensive answer based on the document excerpts above.";

            // Prepare the chat request
            var chatRequest = new OllamaChatRequest
            {
                Model = "llama3.2", // You can change this to any model you have in Ollama
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "system", Content = systemPrompt },
                    new ChatMessage { Role = "user", Content = userPrompt }
                },
                Stream = false,
                Options = new ChatOptions
                {
                    Temperature = 0.3, // Lower temperature for more focused answers
                    NumCtx = 4096,
                    TopK = 40,
                    TopP = 0.9
                }
            };

            var json = JsonSerializer.Serialize(chatRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending chat request to Ollama for answer generation");

            var response = await _httpClient.PostAsync($"{_ollamaBaseUrl}/api/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Ollama chat API returned error. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
                
                return "I apologize, but I encountered an error while generating an answer. Please ensure Ollama is running and try again.";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonSerializer.Deserialize<OllamaChatResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (chatResponse?.Message?.Content != null)
            {
                _logger.LogInformation("Successfully generated answer using RAG");
                return chatResponse.Message.Content.Trim();
            }

            _logger.LogWarning("Ollama returned empty response");
            return "I apologize, but I couldn't generate a proper answer. Please try rephrasing your question.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating answer with Ollama");
            return "I encountered an error while generating an answer. Please ensure Ollama is running with a compatible model and try again.";
        }
    }
}
