using PdfSemanticSearchDemo.Models;
using PdfSemanticSearchDemo.Abstracts;

namespace PdfSemanticSearchDemo.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/document")
            .WithTags("Document")
            .WithOpenApi();

        group.MapPost("/upload", UploadPdf)
            .WithName("UploadPdf")
            .WithSummary("Upload and process a PDF document")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadResponse>(200)
            .Produces<UploadResponse>(400)
            .Produces<UploadResponse>(500);

        group.MapPost("/search", Search)
            .WithName("SearchDocuments")
            .WithSummary("Search documents using semantic similarity")
            .Accepts<SearchRequest>("application/json")
            .Produces<List<SearchResult>>(200)
            .Produces<string>(400)
            .Produces<string>(500);

        group.MapGet("/health", Health)
            .WithName("DocumentHealth")
            .WithSummary("Check document service health")
            .Produces<object>(200);
    }

    private static async Task<IResult> UploadPdf(
        IFormFile file,
        IPdfService pdfService,
        IVectorService vectorService,
        ILogger<Program> logger)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new UploadResponse
                {
                    Success = false,
                    Message = "No file provided"
                });
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new UploadResponse
                {
                    Success = false,
                    Message = "Only PDF files are supported"
                });
            }

            logger.LogInformation("Processing PDF upload: {FileName}", file.FileName);

            // Extract text chunks from PDF
            using var stream = file.OpenReadStream();
            var chunks = await pdfService.ExtractTextFromPdfAsync(stream, file.FileName);

            if (chunks.Count == 0)
            {
                return Results.BadRequest(new UploadResponse
                {
                    Success = false,
                    Message = "No text content found in PDF"
                });
            }

            // Store chunks in vector database
            var success = await vectorService.StoreDocumentChunksAsync(chunks);

            if (!success)
            {
                return Results.Problem(
                    detail: "Failed to store document in vector database",
                    statusCode: 500);
            }

            return Results.Ok(new UploadResponse
            {
                Success = true,
                Message = $"Successfully processed {chunks.Count} text chunks from {file.FileName}",
                ChunksProcessed = chunks.Count
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing PDF upload");
            return Results.Problem(
                detail: "Internal server error occurred while processing the PDF",
                statusCode: 500);
        }
    }

    private static async Task<IResult> Search(
        SearchRequest request,
        IVectorService vectorService,
        ILogger<Program> logger)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return Results.BadRequest("Search query cannot be empty");
            }

            logger.LogInformation("Performing semantic search for: {Query}", request.Query);

            var results = await vectorService.SearchAsync(request.Query, request.Limit, request.Threshold);

            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error performing search");
            return Results.Problem(
                detail: "Internal server error occurred while searching",
                statusCode: 500);
        }
    }

    private static IResult Health()
    {
        return Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}
