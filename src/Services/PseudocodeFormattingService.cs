using System.Text;
using System.Text.RegularExpressions;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Service for formatting pseudocode according to Cambridge International standards
/// </summary>
public class PseudocodeFormattingService : IPseudocodeFormattingService
{
    // Cambridge keywords that should be in UPPER CASE
    private static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "IF", "THEN", "ELSE", "ENDIF", "CASE", "OF", "OTHERWISE", "ENDCASE",
        "FOR", "TO", "STEP", "NEXT", "REPEAT", "UNTIL", "WHILE", "DO", "ENDWHILE",
        "PROCEDURE", "ENDPROCEDURE", "FUNCTION", "RETURN", "ENDFUNCTION", "RETURNS",
        "DECLARE", "CONSTANT", "ARRAY", "TYPE", "ENDTYPE",
        "CALL", "BYREF", "BYVAL", "OUTPUT", "INPUT",
        "OPENFILE", "READFILE", "WRITEFILE", "CLOSEFILE", "EOF",
        "AND", "OR", "NOT", "MOD", "DIV",
        "CLASS", "ENDCLASS", "NEW", "PUBLIC", "PRIVATE", "INHERITS",
        "STRING", "INTEGER", "REAL", "BOOLEAN", "CHAR", "DATE"
    };

    public Task<string> FormatAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Task.FromResult(string.Empty);
        }

        var lines = content.Split('\n');
        var formattedLines = new List<string>();
        var indentLevel = 0;
        const int indentSize = 3;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Skip empty lines but preserve them
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                formattedLines.Add(string.Empty);
                continue;
            }

            // Preserve comments as-is
            if (trimmedLine.StartsWith("//"))
            {
                formattedLines.Add(new string(' ', indentLevel * indentSize) + trimmedLine);
                continue;
            }

            // Decrease indent for closing keywords
            if (IsClosingKeyword(trimmedLine))
            {
                indentLevel = Math.Max(0, indentLevel - 1);
            }

            // Format the line
            var formattedLine = FormatLine(trimmedLine);
            
            // Add proper indentation
            var indentedLine = new string(' ', indentLevel * indentSize) + formattedLine;
            formattedLines.Add(indentedLine);

            // Increase indent for opening keywords
            if (IsOpeningKeyword(trimmedLine))
            {
                indentLevel++;
            }
            // Special case: ELSE should not increase indent but resets it
            else if (IsElseKeyword(trimmedLine))
            {
                // Indent stays the same after ELSE
            }
        }

        return Task.FromResult(string.Join('\n', formattedLines));
    }

    private string FormatLine(string line)
    {
        var result = new StringBuilder();
        var currentWord = new StringBuilder();
        var inString = false;
        var stringChar = '\0';

        for (int i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            // Handle string literals
            if ((ch == '"' || ch == '\'') && (i == 0 || line[i - 1] != '\\'))
            {
                if (!inString)
                {
                    // Start of string
                    if (currentWord.Length > 0)
                    {
                        result.Append(FormatWord(currentWord.ToString()));
                        currentWord.Clear();
                    }
                    inString = true;
                    stringChar = ch;
                    result.Append(ch);
                }
                else if (ch == stringChar)
                {
                    // End of string
                    inString = false;
                    result.Append(ch);
                }
                else
                {
                    result.Append(ch);
                }
                continue;
            }

            if (inString)
            {
                result.Append(ch);
                continue;
            }

            // Handle word boundaries
            if (char.IsLetterOrDigit(ch) || ch == '_')
            {
                currentWord.Append(ch);
            }
            else
            {
                if (currentWord.Length > 0)
                {
                    result.Append(FormatWord(currentWord.ToString()));
                    currentWord.Clear();
                }
                result.Append(ch);
            }
        }

        // Handle last word
        if (currentWord.Length > 0)
        {
            result.Append(FormatWord(currentWord.ToString()));
        }

        return result.ToString();
    }

    private string FormatWord(string word)
    {
        // Check if it's a keyword
        if (Keywords.Contains(word))
        {
            return word.ToUpper();
        }

        // Keep identifiers as-is (assuming they're already in proper camelCase)
        return word;
    }

    private bool IsOpeningKeyword(string line)
    {
        var trimmed = line.Trim();
        
        // Keywords that increase indentation
        return Regex.IsMatch(trimmed, @"^\s*(IF|ELSE|WHILE|FOR|REPEAT|CASE|PROCEDURE|FUNCTION|TYPE|CLASS)\b", RegexOptions.IgnoreCase) ||
               trimmed.ToUpper().EndsWith("THEN") ||
               trimmed.ToUpper().EndsWith("DO");
    }

    private bool IsClosingKeyword(string line)
    {
        var trimmed = line.Trim().ToUpper();
        
        // Keywords that decrease indentation
        return trimmed.StartsWith("ENDIF") ||
               trimmed.StartsWith("ENDWHILE") ||
               trimmed.StartsWith("NEXT") ||
               trimmed.StartsWith("UNTIL") ||
               trimmed.StartsWith("ENDCASE") ||
               trimmed.StartsWith("ENDPROCEDURE") ||
               trimmed.StartsWith("ENDFUNCTION") ||
               trimmed.StartsWith("ENDTYPE") ||
               trimmed.StartsWith("ENDCLASS") ||
               trimmed.StartsWith("ELSE");
    }

    private bool IsElseKeyword(string line)
    {
        var trimmed = line.Trim().ToUpper();
        return trimmed.StartsWith("ELSE") || trimmed.StartsWith("OTHERWISE");
    }
}

public interface IPseudocodeFormattingService
{
    Task<string> FormatAsync(string content);
}
