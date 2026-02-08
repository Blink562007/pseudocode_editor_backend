using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Main business logic service for pseudocode document management
/// Orchestrates validation, formatting, and CRUD operations
/// </summary>
public class PseudocodeService : IPseudocodeService
{
    // In-memory storage for demo purposes
    private static readonly List<PseudocodeDocument> Documents = new();
    
    private readonly IPseudocodeValidationService _validationService;
    private readonly IPseudocodeFormattingService _formattingService;

    public PseudocodeService(
        IPseudocodeValidationService validationService,
        IPseudocodeFormattingService formattingService)
    {
        _validationService = validationService;
        _formattingService = formattingService;
    }

    public Task<IEnumerable<PseudocodeDocument>> GetAllDocumentsAsync()
    {
        return Task.FromResult<IEnumerable<PseudocodeDocument>>(Documents);
    }

    public Task<PseudocodeDocument?> GetDocumentByIdAsync(string id)
    {
        var document = Documents.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(document);
    }

    public async Task<PseudocodeDocument> CreateDocumentAsync(CreatePseudocodeRequest request)
    {
        // Process the content: validate and format
        var processedContent = await ProcessContentAsync(request.Content);
        
        var document = new PseudocodeDocument
        {
            Title = request.Title?.Trim() ?? "Untitled",
            Content = processedContent,
            Language = request.Language ?? "pseudocode"
        };
        
        Documents.Add(document);
        return document;
    }

    public async Task<PseudocodeDocument?> UpdateDocumentAsync(string id, UpdatePseudocodeRequest request)
    {
        var document = Documents.FirstOrDefault(d => d.Id == id);
        if (document == null)
        {
            return null;
        }

        // Process the content: validate and format
        var processedContent = await ProcessContentAsync(request.Content);
        
        document.Title = request.Title?.Trim() ?? document.Title;
        document.Content = processedContent;
        document.Language = request.Language ?? document.Language;
        document.UpdatedAt = DateTime.UtcNow;

        return document;
    }

    public Task<bool> DeleteDocumentAsync(string id)
    {
        var document = Documents.FirstOrDefault(d => d.Id == id);
        if (document == null)
        {
            return Task.FromResult(false);
        }

        Documents.Remove(document);
        return Task.FromResult(true);
    }

    public async Task<ValidationResult> ValidateContentAsync(string content)
    {
        return await _validationService.ValidateAsync(content);
    }

    public async Task<string> FormatContentAsync(string content)
    {
        return await _formattingService.FormatAsync(content);
    }

    /// <summary>
    /// Process content: validate and auto-format according to Cambridge standards
    /// This is called when creating or updating documents from the frontend
    /// </summary>
    private async Task<string> ProcessContentAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        // Step 1: Validate the content
        var validationResult = await _validationService.ValidateAsync(content);
        
        // Step 2: Auto-format the content to meet Cambridge standards
        var formattedContent = await _formattingService.FormatAsync(content);
        
        // For now, we apply formatting regardless of validation errors
        // In a production system, you might want to:
        // - Return validation errors to the client
        // - Allow users to choose whether to auto-format
        // - Store validation warnings with the document
        
        return formattedContent;
    }
}
