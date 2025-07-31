using PdfSemanticSearchDemo.Models;
using PdfSemanticSearchDemo.Abstracts;

namespace PdfSemanticSearchDemo.Endpoints;

public static class RagEndpoints
{
    public static void MapRagEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/rag")
            .WithTags("RAG")
            .WithOpenApi();

        group.MapPost("/search", SearchWithRag)
            .WithName("RagSearch")
            .WithSummary("Search using Retrieval-Augmented Generation")
            .Accepts<RagSearchRequest>("application/json")
            .Produces<RagSearchResponse>(200)
            .Produces<RagSearchResponse>(400)
            .Produces<RagSearchResponse>(500);

        group.MapGet("/health", Health)
            .WithName("RagHealth")
            .WithSummary("Check RAG service health")
            .Produces<object>(200);
    }

    private static async Task<IResult> SearchWithRag(
        RagSearchRequest request,
        IRagService ragService,
        ILogger<Program> logger)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return Results.BadRequest(new RagSearchResponse
                {
                    Success = false,
                    ErrorMessage = "Search query cannot be empty",
                    Query = request.Query
                });
            }

            logger.LogInformation("RAG search request received for query: {Query}", request.Query);

            var response = await ragService.SearchWithRagAsync(request);

            if (!response.Success)
            {
                return Results.Problem(
                    detail: response.ErrorMessage,
                    statusCode: 500);
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing RAG search request");
            return Results.Problem(
                detail: "Internal server error occurred while processing your request",
                statusCode: 500);
        }
    }

    private static IResult Health()
    {
        return Results.Ok(new { Status = "Healthy", Service = "RAG", Timestamp = DateTime.UtcNow });
    }
}
