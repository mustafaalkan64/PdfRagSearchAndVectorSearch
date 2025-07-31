namespace PdfSemanticSearchDemo.Abstracts;

public interface IOllamaService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}
