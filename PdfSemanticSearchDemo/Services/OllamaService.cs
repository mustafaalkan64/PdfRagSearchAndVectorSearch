using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PdfSemanticSearchDemo.Abstracts;

namespace PdfSemanticSearchDemo.Services;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaService> _logger;
    private readonly string _baseUrl;

    public OllamaService(HttpClient httpClient, ILogger<OllamaService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration.GetValue<string>("Ollama:BaseUrl") ?? "http://localhost:11434";
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var request = new
            {
                model = "nomic-embed-text", // You can change this to any embedding model available in Ollama
                prompt = text
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var envValue = Environment.GetEnvironmentVariable("OLLAMA_API_URL");
            var ollamaUrl = !string.IsNullOrEmpty(envValue) ? envValue : _baseUrl;

            var response = await _httpClient.PostAsync($"{ollamaUrl}/api/embeddings", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to generate embedding. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, await response.Content.ReadAsStringAsync());
                throw new HttpRequestException($"Ollama API returned {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var embeddingResponse = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseContent);

            if (embeddingResponse?.Embedding == null)
            {
                throw new InvalidOperationException("No embedding returned from Ollama");
            }

            return embeddingResponse.Embedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text: {Text}", text[..Math.Min(text.Length, 100)]);
            throw;
        }
    }

    private class OllamaEmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; }
    }
}
