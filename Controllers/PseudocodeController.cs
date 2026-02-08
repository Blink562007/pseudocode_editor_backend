using Microsoft.AspNetCore.Mvc;
using PseudocodeEditorAPI.Models;
using PseudocodeEditorAPI.Services;

namespace PseudocodeEditorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PseudocodeController : ControllerBase
{
    private readonly IPseudocodeService _pseudocodeService;

    public PseudocodeController(IPseudocodeService pseudocodeService)
    {
        _pseudocodeService = pseudocodeService;
    }

    /// <summary>
    /// Get all pseudocode documents
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PseudocodeDocument>>> GetAll()
    {
        var documents = await _pseudocodeService.GetAllDocumentsAsync();
        return Ok(documents);
    }

    /// <summary>
    /// Get a specific pseudocode document by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PseudocodeDocument>> GetById(string id)
    {
        var document = await _pseudocodeService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }
        return Ok(document);
    }

    /// <summary>
    /// Create a new pseudocode document
    /// Content is automatically validated and formatted according to Cambridge standards
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PseudocodeDocument>> Create([FromBody] CreatePseudocodeRequest request)
    {
        var document = await _pseudocodeService.CreateDocumentAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    /// <summary>
    /// Update an existing pseudocode document
    /// Content is automatically validated and formatted according to Cambridge standards
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PseudocodeDocument>> Update(string id, [FromBody] UpdatePseudocodeRequest request)
    {
        var document = await _pseudocodeService.UpdateDocumentAsync(id, request);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }
        return Ok(document);
    }

    /// <summary>
    /// Delete a pseudocode document
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var success = await _pseudocodeService.DeleteDocumentAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Document not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Validate pseudocode content without saving
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ValidationResult>> Validate([FromBody] ValidateContentRequest request)
    {
        var result = await _pseudocodeService.ValidateContentAsync(request.Content);
        return Ok(result);
    }

    /// <summary>
    /// Format pseudocode content according to Cambridge standards without saving
    /// </summary>
    [HttpPost("format")]
    public async Task<ActionResult<FormatContentResponse>> Format([FromBody] FormatContentRequest request)
    {
        var formattedContent = await _pseudocodeService.FormatContentAsync(request.Content);
        return Ok(new FormatContentResponse { FormattedContent = formattedContent });
    }
}
