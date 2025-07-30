using PdfSemanticSearchDemo.Models;

namespace PdfSemanticSearchDemo.Services;

public interface IRagService
{
    Task<RagSearchResponse> SearchWithRagAsync(RagSearchRequest request);
    Task<string> GenerateAnswerAsync(string query, List<SearchResult> context);
}
