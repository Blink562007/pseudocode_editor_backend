using System.Text.RegularExpressions;
using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Service for validating pseudocode against Cambridge International standards
/// </summary>
public class PseudocodeValidationService : IPseudocodeValidationService
{
    // Cambridge keywords that should be in UPPER CASE
    private static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "IF", "THEN", "ELSE", "ENDIF", "CASE", "OF", "OTHERWISE", "ENDCASE",
        "FOR", "TO", "STEP", "NEXT", "REPEAT", "UNTIL", "WHILE", "DO", "ENDWHILE",
        "PROCEDURE", "ENDPROCEDURE", "FUNCTION", "RETURNS", "ENDFUNCTION",
        "DECLARE", "CONSTANT", "ARRAY", "TYPE", "ENDTYPE",
        "CALL", "BYREF", "BYVAL", "OUTPUT", "INPUT",
        "OPENFILE", "READFILE", "WRITEFILE", "CLOSEFILE", "EOF",
        "AND", "OR", "NOT", "MOD", "DIV",
        "CLASS", "ENDCLASS", "NEW", "PUBLIC", "PRIVATE", "INHERITS"
    };

    public Task<ValidationResult> ValidateAsync(string content)
    {
        var result = new ValidationResult { IsValid = true };
        
        if (string.IsNullOrWhiteSpace(content))
        {
            return Task.FromResult(result);
        }

        var lines = content.Split('\n');
        
        for (int i = 0; i < lines.Length; i++)
        {
            var lineNumber = i + 1;
            var line = lines[i];
            
            // Skip empty lines and comments
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                continue;
            
            // Check for keywords in wrong case
            ValidateKeywordCase(line, lineNumber, result);
            
            // Check for proper identifier naming (camelCase)
            ValidateIdentifierCase(line, lineNumber, result);
            
            // Check for proper indentation (multiples of 3 spaces)
            ValidateIndentation(line, lineNumber, result);
            
            // Check for common syntax issues
            ValidateSyntax(line, lineNumber, result);
        }

        result.IsValid = result.Errors.Count == 0;
        return Task.FromResult(result);
    }

    private void ValidateKeywordCase(string line, int lineNumber, ValidationResult result)
    {
        // Match potential keywords (word boundaries)
        var words = Regex.Matches(line, @"\b[A-Za-z]+\b");
        
        foreach (Match match in words)
        {
            var word = match.Value;
            
            // Check if it's a keyword
            if (Keywords.Contains(word))
            {
                // Check if it's all uppercase
                if (word != word.ToUpper())
                {
                    result.Warnings.Add(new ValidationWarning
                    {
                        LineNumber = lineNumber,
                        Message = $"Keyword '{word}' should be in UPPER CASE: {word.ToUpper()}",
                        Code = "KEYWORD_CASE"
                    });
                }
            }
        }
    }

    private void ValidateIdentifierCase(string line, int lineNumber, ValidationResult result)
    {
        // Check for identifiers that should use camelCase/PascalCase
        // This is a warning, not an error
        var identifierPattern = @"\b([a-z_]+[a-z_]+)\b";
        var matches = Regex.Matches(line, identifierPattern);
        
        foreach (Match match in matches)
        {
            var identifier = match.Value;
            
            // Skip if it's a keyword or all lowercase single word
            if (Keywords.Contains(identifier) || identifier.Length < 3)
                continue;
            
            // Check if it contains underscore (should use camelCase instead)
            if (identifier.Contains('_'))
            {
                result.Warnings.Add(new ValidationWarning
                {
                    LineNumber = lineNumber,
                    Message = $"Identifier '{identifier}' should use camelCase instead of snake_case",
                    Code = "IDENTIFIER_CASE"
                });
            }
        }
    }

    private void ValidateIndentation(string line, int lineNumber, ValidationResult result)
    {
        if (string.IsNullOrEmpty(line) || !char.IsWhiteSpace(line[0]))
            return;
        
        var leadingSpaces = line.TakeWhile(c => c == ' ').Count();
        
        // Indentation should be in multiples of 3
        if (leadingSpaces % 3 != 0)
        {
            result.Warnings.Add(new ValidationWarning
            {
                LineNumber = lineNumber,
                Message = $"Indentation should be in multiples of 3 spaces (found {leadingSpaces})",
                Code = "INDENTATION"
            });
        }
    }

    private void ValidateSyntax(string line, int lineNumber, ValidationResult result)
    {
        var trimmedLine = line.Trim();
        
        // Check for missing THEN after IF
        if (Regex.IsMatch(trimmedLine, @"\bIF\b.*\bTHEN\b") == false && 
            Regex.IsMatch(trimmedLine, @"^\s*IF\b") && 
            !trimmedLine.EndsWith("THEN"))
        {
            // This is just a warning as THEN might be on next line
            result.Warnings.Add(new ValidationWarning
            {
                LineNumber = lineNumber,
                Message = "IF statement should include THEN on the same or next line",
                Code = "MISSING_THEN"
            });
        }
        
        // Check for unmatched brackets
        var openParens = trimmedLine.Count(c => c == '(');
        var closeParens = trimmedLine.Count(c => c == ')');
        var openBrackets = trimmedLine.Count(c => c == '[');
        var closeBrackets = trimmedLine.Count(c => c == ']');
        
        if (openParens != closeParens)
        {
            result.Errors.Add(new ValidationError
            {
                LineNumber = lineNumber,
                Message = "Unmatched parentheses",
                Code = "UNMATCHED_PARENS"
            });
        }
        
        if (openBrackets != closeBrackets)
        {
            result.Errors.Add(new ValidationError
            {
                LineNumber = lineNumber,
                Message = "Unmatched brackets",
                Code = "UNMATCHED_BRACKETS"
            });
        }
    }
}

public interface IPseudocodeValidationService
{
    Task<ValidationResult> ValidateAsync(string content);
}
