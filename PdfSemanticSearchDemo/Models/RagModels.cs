namespace PdfSemanticSearchDemo.Models;

public class RagSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 5;
    public float Threshold { get; set; } = 0.3f;
    public bool IncludeSourceDocuments { get; set; } = true;
}

public class RagSearchResponse
{
    public string GeneratedAnswer { get; set; } = string.Empty;
    public List<SearchResult> SourceDocuments { get; set; } = new();
    public string Query { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public double ResponseTimeMs { get; set; }
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty; // "system", "user", "assistant"
    public string Content { get; set; } = string.Empty;
}

public class OllamaChatRequest
{
    public string Model { get; set; } = "llama3.2";
    public List<ChatMessage> Messages { get; set; } = new();
    public bool Stream { get; set; } = false;
    public ChatOptions? Options { get; set; }
}

public class ChatOptions
{
    public double Temperature { get; set; } = 0.7;
    public int NumCtx { get; set; } = 4096;
    public int TopK { get; set; } = 40;
    public double TopP { get; set; } = 0.9;
}

public class OllamaChatResponse
{
    public ChatMessage Message { get; set; } = new();
    public bool Done { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}
