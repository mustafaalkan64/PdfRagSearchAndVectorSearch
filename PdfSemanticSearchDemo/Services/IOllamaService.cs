namespace PdfSemanticSearchDemo.Services;

public interface IOllamaService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}
