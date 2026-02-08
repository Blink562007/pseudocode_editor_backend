using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Interface for pseudocode document business logic
/// </summary>
public interface IPseudocodeService
{
    /// <summary>
    /// Get all pseudocode documents
    /// </summary>
    Task<IEnumerable<PseudocodeDocument>> GetAllDocumentsAsync();
    
    /// <summary>
    /// Get a specific document by ID
    /// </summary>
    Task<PseudocodeDocument?> GetDocumentByIdAsync(string id);
    
    /// <summary>
    /// Create a new pseudocode document with validation and formatting
    /// </summary>
    Task<PseudocodeDocument> CreateDocumentAsync(CreatePseudocodeRequest request);
    
    /// <summary>
    /// Update an existing pseudocode document with validation and formatting
    /// </summary>
    Task<PseudocodeDocument?> UpdateDocumentAsync(string id, UpdatePseudocodeRequest request);
    
    /// <summary>
    /// Delete a pseudocode document
    /// </summary>
    Task<bool> DeleteDocumentAsync(string id);
    
    /// <summary>
    /// Validate pseudocode content against Cambridge standards
    /// </summary>
    Task<ValidationResult> ValidateContentAsync(string content);
    
    /// <summary>
    /// Format pseudocode content according to Cambridge style guide
    /// </summary>
    Task<string> FormatContentAsync(string content);
}
