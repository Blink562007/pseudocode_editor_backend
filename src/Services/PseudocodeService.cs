using PseudocodeEditorAPI.Models;
using PseudocodeEditorAPI.Data.Repositories;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Main business logic service for pseudocode document management
/// Orchestrates validation, formatting, and CRUD operations
/// </summary>
public class PseudocodeService : IPseudocodeService
{
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly IPseudocodeValidationService _validationService;
    private readonly IPseudocodeFormattingService _formattingService;

    public PseudocodeService(
        IPseudocodeDocumentRepository repository,
        IPseudocodeValidationService validationService,
        IPseudocodeFormattingService formattingService)
    {
        _repository = repository;
        _validationService = validationService;
        _formattingService = formattingService;
    }

    public async Task<IEnumerable<PseudocodeDocument>> GetAllDocumentsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<PseudocodeDocument?> GetDocumentByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<PseudocodeDocument> CreateDocumentAsync(CreatePseudocodeRequest request)
    {
        // Process the content: validate and format
        var processedContent = await ProcessContentAsync(request.Content);
        
        var trimmedTitle = request.Title?.Trim();
        var document = new PseudocodeDocument
        {
            Title = string.IsNullOrWhiteSpace(trimmedTitle) ? "Untitled" : trimmedTitle,
            Content = processedContent,
            Language = request.Language ?? "pseudocode"
        };

        return await _repository.CreateAsync(document);
    }

    public async Task<PseudocodeDocument?> UpdateDocumentAsync(string id, UpdatePseudocodeRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            return null;
        }

        // Update title with default to "Untitled" if empty
        var trimmedTitle = request.Title?.Trim();
        existing.Title = string.IsNullOrWhiteSpace(trimmedTitle) ? "Untitled" : trimmedTitle;

        // Process content only if it has changed (optimization for rename-only updates)
        var contentChanged = request.Content != existing.Content;
        if (contentChanged)
        {
            existing.Content = await ProcessContentAsync(request.Content);
        }

        existing.Language = request.Language ?? existing.Language;
        existing.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteDocumentAsync(string id)
    {
        return await _repository.DeleteAsync(id);
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
        _ = await _validationService.ValidateAsync(content);
        
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
