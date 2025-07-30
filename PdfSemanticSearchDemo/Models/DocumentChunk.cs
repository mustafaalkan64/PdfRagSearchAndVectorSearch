namespace PdfSemanticSearchDemo.Models;

public class DocumentChunk
{
    public ulong Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public int ChunkIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public float[]? Embedding { get; set; }
}

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
    public float Threshold { get; set; } = 0.7f;
}

public class SearchResult
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public float Score { get; set; }
}

public class UploadResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ChunksProcessed { get; set; }
}
