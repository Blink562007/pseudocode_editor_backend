using Microsoft.AspNetCore.Mvc;
using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PseudocodeController : ControllerBase
{
    // In-memory storage for demo purposes
    private static readonly List<PseudocodeDocument> Documents = new();

    /// <summary>
    /// Get all pseudocode documents
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<PseudocodeDocument>> GetAll()
    {
        return Ok(Documents);
    }

    /// <summary>
    /// Get a specific pseudocode document by ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<PseudocodeDocument> GetById(string id)
    {
        var document = Documents.FirstOrDefault(d => d.Id == id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }
        return Ok(document);
    }

    /// <summary>
    /// Create a new pseudocode document
    /// </summary>
    [HttpPost]
    public ActionResult<PseudocodeDocument> Create([FromBody] CreatePseudocodeRequest request)
    {
        var document = new PseudocodeDocument
        {
            Title = request.Title,
            Content = request.Content,
            Language = request.Language
        };
        
        Documents.Add(document);
        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    /// <summary>
    /// Update an existing pseudocode document
    /// </summary>
    [HttpPut("{id}")]
    public ActionResult<PseudocodeDocument> Update(string id, [FromBody] UpdatePseudocodeRequest request)
    {
        var document = Documents.FirstOrDefault(d => d.Id == id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }

        document.Title = request.Title;
        document.Content = request.Content;
        document.Language = request.Language;
        document.UpdatedAt = DateTime.UtcNow;

        return Ok(document);
    }

    /// <summary>
    /// Delete a pseudocode document
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var document = Documents.FirstOrDefault(d => d.Id == id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }

        Documents.Remove(document);
        return NoContent();
    }
}
