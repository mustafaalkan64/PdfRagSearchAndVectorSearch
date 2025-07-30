using PdfSemanticSearchDemo.Models;

namespace PdfSemanticSearchDemo.Services;

public interface IVectorService
{
    Task InitializeAsync();
    Task<bool> StoreDocumentChunksAsync(List<DocumentChunk> chunks);
    Task<List<SearchResult>> SearchAsync(string query, int limit = 10, float threshold = 0.7f);
}
