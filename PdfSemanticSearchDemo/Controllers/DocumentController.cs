using Microsoft.AspNetCore.Mvc;
using PdfSemanticSearchDemo.Models;
using PdfSemanticSearchDemo.Services;

namespace PdfSemanticSearchDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IPdfService _pdfService;
    private readonly IVectorService _vectorService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(IPdfService pdfService, IVectorService vectorService, ILogger<DocumentController> logger)
    {
        _pdfService = pdfService;
        _vectorService = vectorService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<UploadResponse>> UploadPdf(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new UploadResponse
                {
                    Success = false,
                    Message = "No file provided"
                });
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new UploadResponse
                {
                    Success = false,
                    Message = "Only PDF files are supported"
                });
            }

            _logger.LogInformation("Processing PDF upload: {FileName}", file.FileName);

            // Extract text chunks from PDF
            using var stream = file.OpenReadStream();
            var chunks = await _pdfService.ExtractTextFromPdfAsync(stream, file.FileName);

            if (chunks.Count == 0)
            {
                return BadRequest(new UploadResponse
                {
                    Success = false,
                    Message = "No text content found in PDF"
                });
            }

            // Store chunks in vector database
            var success = await _vectorService.StoreDocumentChunksAsync(chunks);

            if (!success)
            {
                return StatusCode(500, new UploadResponse
                {
                    Success = false,
                    Message = "Failed to store document in vector database"
                });
            }

            return Ok(new UploadResponse
            {
                Success = true,
                Message = $"Successfully processed {chunks.Count} text chunks from {file.FileName}",
                ChunksProcessed = chunks.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PDF upload");
            return StatusCode(500, new UploadResponse
            {
                Success = false,
                Message = "Internal server error occurred while processing the PDF"
            });
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<SearchResult>>> Search([FromBody] SearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest("Search query cannot be empty");
            }

            _logger.LogInformation("Performing semantic search for: {Query}", request.Query);

            var results = await _vectorService.SearchAsync(request.Query, request.Limit, request.Threshold);

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search");
            return StatusCode(500, "Internal server error occurred while searching");
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}
