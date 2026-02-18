using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Service for executing Cambridge-style pseudocode
/// </summary>
public interface IPseudocodeExecutionService
{
    /// <summary>
    /// Execute pseudocode and return output events and errors
    /// </summary>
    Task<ExecuteCodeResponse> ExecuteAsync(string content);
}
