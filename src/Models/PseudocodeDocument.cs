namespace PseudocodeEditorAPI.Models;

public class PseudocodeDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Language { get; set; } = "pseudocode";
}

public class CreatePseudocodeRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Language { get; set; } = "pseudocode";
}

public class UpdatePseudocodeRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Language { get; set; } = "pseudocode";
}

public class ValidateContentRequest
{
    public string Content { get; set; } = string.Empty;
}

public class FormatContentRequest
{
    public string Content { get; set; } = string.Empty;
}

public class FormatContentResponse
{
    public string FormattedContent { get; set; } = string.Empty;
}

public class ExecuteCodeRequest
{
    public string Content { get; set; } = string.Empty;
    public string Language { get; set; } = "pseudocode";
}

public class ExecuteCodeResponse
{
    public bool Success { get; set; }
    public List<ExecutionEvent> Events { get; set; } = new();
    public double ExecutionTimeMs { get; set; }
}

public class ExecutionEvent
{
    public string Kind { get; set; } = string.Empty; // "output", "error", "system"
    public string Text { get; set; } = string.Empty;
    public int? Line { get; set; }
}
