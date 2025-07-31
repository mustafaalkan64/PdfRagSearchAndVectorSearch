using PdfSemanticSearchDemo.Models;

namespace PdfSemanticSearchDemo.Abstracts;

public interface IRagService
{
    Task<RagSearchResponse> SearchWithRagAsync(RagSearchRequest request);
    Task<string> GenerateAnswerAsync(string query, List<SearchResult> context);
}
