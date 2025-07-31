using Qdrant.Client;
using Qdrant.Client.Grpc;
using PdfSemanticSearchDemo.Models;
using PdfSemanticSearchDemo.Utilities;
using System.Text.Json.Serialization;
using System.Text.Json;
using PdfSemanticSearchDemo.Abstracts;

namespace PdfSemanticSearchDemo.Services;

public class QdrantVectorService : IVectorService
{
    private readonly QdrantClient _qdrantClient;
    private readonly IOllamaService _ollamaService;
    private readonly ILogger<QdrantVectorService> _logger;
    private readonly string _collectionName = "pdf_documents";

    public QdrantVectorService(QdrantClient qdrantClient, IOllamaService ollamaService, ILogger<QdrantVectorService> logger)
    {
        _qdrantClient = qdrantClient;
        _ollamaService = ollamaService;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Check if collection exists
            var collections = await _qdrantClient.ListCollectionsAsync();
            var collectionExists = collections.Any(c => c == _collectionName);

            if (!collectionExists)
            {
                _logger.LogInformation("Creating collection: {CollectionName}", _collectionName);
                
                // Create collection with vector configuration
                await _qdrantClient.CreateCollectionAsync(_collectionName, new VectorParams
                {
                    Size = 768, // nomic-embed-text embedding size
                    Distance = Distance.Cosine
                });

                
                _logger.LogInformation("Collection created successfully: {CollectionName}", _collectionName);
            }
            else
            {
                _logger.LogInformation("Collection already exists: {CollectionName}", _collectionName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Qdrant collection");
            throw;
        }
    }

    public async Task<bool> StoreDocumentChunksAsync(List<DocumentChunk> chunks)
    {
        try
        {
            var points = new List<PointStruct>();

            foreach (var chunk in chunks)
            {
                // Generate embedding for the chunk content
                var embedding = await _ollamaService.GenerateEmbeddingAsync(chunk.Content);
                chunk.Embedding = embedding;
                var text = chunk.Content; // Use the content directly for payload

                // Create point for Qdrant
                var point = new PointStruct
                {
                    Id = RandomHelper.GetRandomULong(),
                    Vectors = embedding,
                    Payload =
                    {
                        ["content"] = chunk.Content,
                        ["fileName"] = chunk.FileName,
                        ["pageNumber"] = chunk.PageNumber,
                        ["chunkIndex"] = chunk.ChunkIndex,
                        ["createdAt"] = chunk.CreatedAt.ToString("O")
                    }
                };

                points.Add(point);
            }

            // Upsert points to Qdrant
            await _qdrantClient.UpsertAsync(_collectionName, points);

            _logger.LogInformation("Successfully stored {Count} document chunks", chunks.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing document chunks");
            return false;
        }
    }

    public async Task<List<SearchResult>> SearchAsync(string query, int limit = 10, float threshold = 0.7f)
    {
        try
        {
            _logger.LogInformation("Starting search for query: {Query} with limit: {Limit}, threshold: {Threshold}", query, limit, threshold);
            
            // Check if collection exists and has data
            var collectionInfo = await _qdrantClient.GetCollectionInfoAsync(_collectionName);
            _logger.LogInformation("Collection {CollectionName} has {PointsCount} points", _collectionName, collectionInfo.PointsCount);
            
            if (collectionInfo.PointsCount == 0)
            {
                _logger.LogWarning("Collection {CollectionName} is empty. No documents have been uploaded yet.", _collectionName);
                return new List<SearchResult>();
            }

            // Generate embedding for the search query
            var queryEmbedding = await _ollamaService.GenerateEmbeddingAsync(query);
            _logger.LogInformation("Generated embedding for query. Embedding size: {Size}", queryEmbedding?.Length ?? 0);
            
            if (queryEmbedding == null || queryEmbedding.Length == 0)
            {
                _logger.LogError("Failed to generate embedding for query: {Query}", query);
                throw new InvalidOperationException("Failed to generate embedding for search query");
            }

            // Search in Qdrant with lower threshold for debugging
            var searchResult = await _qdrantClient.SearchAsync(_collectionName, queryEmbedding, limit: (ulong)limit, scoreThreshold: 0.0f);
            
            _logger.LogInformation("Qdrant search returned {ResultCount} results", searchResult?.Count ?? 0);
            
            if (searchResult == null)
            {
                _logger.LogError("Qdrant search returned null for collection: {CollectionName}", _collectionName);
                return new List<SearchResult>();
            }

            var results = new List<SearchResult>();

            foreach (var point in searchResult)
            {
                try
                {
                    _logger.LogDebug("Processing search result point {PointId} with score {Score}", point.Id, point.Score);
                    
                    // Extract values from payload with proper type handling
                    var content = GetPayloadValue(point.Payload, "content");
                    var fileName = GetPayloadValue(point.Payload, "fileName");
                    var pageNumberStr = GetPayloadValue(point.Payload, "pageNumber");
                    
                    // Parse page number safely
                    int.TryParse(pageNumberStr, out int pageNumber);

                    var result = new SearchResult
                    {
                        Id = point.Id.ToString(),
                        Content = content,
                        FileName = fileName,
                        PageNumber = pageNumber,
                        Score = point.Score
                    };

                    results.Add(result);
                    _logger.LogDebug("Successfully processed point {PointId}: {FileName}, Page {PageNumber}", point.Id, fileName, pageNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing search result point {PointId}", point.Id);
                }
            }

            _logger.LogInformation("Search completed. Found {Count} results for query: {Query}", results.Count, query);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching documents for query: {Query}", query);
            throw;
        }
    }

    private string GetPayloadValue(IDictionary<string, Value> payload, string key)
    {
        if (payload.TryGetValue(key, out var value))
        {
            return value.KindCase switch
            {
                Value.KindOneofCase.StringValue => value.StringValue,
                Value.KindOneofCase.IntegerValue => value.IntegerValue.ToString(),
                Value.KindOneofCase.DoubleValue => value.DoubleValue.ToString(),
                Value.KindOneofCase.BoolValue => value.BoolValue.ToString(),
                _ => value.ToString()
            };
        }
        return string.Empty;
    }
}
