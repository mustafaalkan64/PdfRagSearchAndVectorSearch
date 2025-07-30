using Microsoft.AspNetCore.Mvc;
using PdfSemanticSearchDemo.Models;
using PdfSemanticSearchDemo.Services;

namespace PdfSemanticSearchDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RagController : ControllerBase
{
    private readonly IRagService _ragService;
    private readonly ILogger<RagController> _logger;

    public RagController(IRagService ragService, ILogger<RagController> logger)
    {
        _ragService = ragService;
        _logger = logger;
    }

    [HttpPost("search")]
    public async Task<ActionResult<RagSearchResponse>> SearchWithRag([FromBody] RagSearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new RagSearchResponse
                {
                    Success = false,
                    ErrorMessage = "Search query cannot be empty",
                    Query = request.Query
                });
            }

            _logger.LogInformation("RAG search request received for query: {Query}", request.Query);

            var response = await _ragService.SearchWithRagAsync(request);

            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RAG search request");
            return StatusCode(500, new RagSearchResponse
            {
                Success = false,
                ErrorMessage = "Internal server error occurred while processing your request",
                Query = request.Query
            });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Service = "RAG", Timestamp = DateTime.UtcNow });
    }
}
