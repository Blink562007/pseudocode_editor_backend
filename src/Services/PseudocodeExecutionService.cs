using PseudocodeEditorAPI.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PseudocodeEditorAPI.Services;

/// <summary>
/// Executes Cambridge-style pseudocode with OUTPUT support and runtime error tracking
/// </summary>
public class PseudocodeExecutionService : IPseudocodeExecutionService
{
    private const int MaxExecutionSteps = 10000;
    private const int MaxExecutionTimeMs = 5000;
    
    private Dictionary<string, object> _variables = new();
    private HashSet<string> _constants = new();
    private List<ExecutionEvent> _events = new();
    private int _currentLine = 0;
    private int _stepCount = 0;

    public async Task<ExecuteCodeResponse> ExecuteAsync(string content)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Reset state
            _variables = new Dictionary<string, object>();
            _constants = new HashSet<string>();
            _events = new List<ExecutionEvent>();
            _currentLine = 0;
            _stepCount = 0;

            if (string.IsNullOrWhiteSpace(content))
            {
                _events.Add(new ExecutionEvent
                {
                    Kind = "system",
                    Text = "No code to execute"
                });
                
                return new ExecuteCodeResponse
                {
                    Success = false,
                    Events = _events,
                    ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds
                };
            }

            // Split into lines and execute
            var lines = content.Split('\n', StringSplitOptions.None);
            
            for (int i = 0; i < lines.Length; i++)
            {
                _currentLine = i + 1;
                _stepCount++;
                
                // Check safety limits
                if (_stepCount > MaxExecutionSteps)
                {
                    throw new RuntimeException("Execution step limit exceeded (possible infinite loop)", _currentLine);
                }
                
                if (stopwatch.ElapsedMilliseconds > MaxExecutionTimeMs)
                {
                    throw new RuntimeException("Execution time limit exceeded", _currentLine);
                }
                
                var line = lines[i].Trim();
                
                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("#"))
                {
                    continue;
                }

                await ExecuteLineAsync(line, _currentLine);
            }

            stopwatch.Stop();

            _events.Add(new ExecutionEvent
            {
                Kind = "system",
                Text = $"Program finished (execution time: {stopwatch.ElapsedMilliseconds}ms)"
            });

            return new ExecuteCodeResponse
            {
                Success = true,
                Events = _events,
                ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
        catch (RuntimeException ex)
        {
            stopwatch.Stop();
            
            _events.Add(new ExecutionEvent
            {
                Kind = "error",
                Text = $"Line {ex.Line}: RuntimeError — {ex.Message}",
                Line = ex.Line
            });

            return new ExecuteCodeResponse
            {
                Success = false,
                Events = _events,
                ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _events.Add(new ExecutionEvent
            {
                Kind = "error",
                Text = $"Line {_currentLine}: RuntimeError — {ex.Message}",
                Line = _currentLine
            });

            return new ExecuteCodeResponse
            {
                Success = false,
                Events = _events,
                ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }

    private async Task ExecuteLineAsync(string line, int lineNumber)
    {
        await Task.CompletedTask; // Make it async for future extensibility

        // OUTPUT statement
        var outputMatch = Regex.Match(line, @"^OUTPUT\s+(.+)$", RegexOptions.IgnoreCase);
        if (outputMatch.Success)
        {
            var expression = outputMatch.Groups[1].Value.Trim();
            var result = EvaluateExpression(expression, lineNumber);
            
            _events.Add(new ExecutionEvent
            {
                Kind = "output",
                Text = result.ToString() ?? "",
                Line = lineNumber
            });
            return;
        }

        // DECLARE statement
        var declareMatch = Regex.Match(line, @"^DECLARE\s+(\w+)\s*:\s*(\w+)$", RegexOptions.IgnoreCase);
        if (declareMatch.Success)
        {
            var varName = declareMatch.Groups[1].Value;
            var varType = declareMatch.Groups[2].Value.ToUpperInvariant();
            
            if (_variables.ContainsKey(varName))
            {
                throw new RuntimeException($"Variable '{varName}' is already declared", lineNumber);
            }
            
            // Initialize with default value based on type
            object defaultValue = varType switch
            {
                "INTEGER" => 0,
                "REAL" => 0.0,
                "STRING" => "",
                "BOOLEAN" => false,
                "CHAR" => '\0',
                _ => throw new RuntimeException($"Unknown type '{varType}'", lineNumber)
            };
            
            _variables[varName] = defaultValue;
            return;
        }

        // CONSTANT statement
        var constantMatch = Regex.Match(line, @"^CONSTANT\s+(\w+)\s*=\s*(.+)$", RegexOptions.IgnoreCase);
        if (constantMatch.Success)
        {
            var constName = constantMatch.Groups[1].Value;
            var valueExpr = constantMatch.Groups[2].Value.Trim();
            
            if (_variables.ContainsKey(constName))
            {
                throw new RuntimeException($"'{constName}' is already declared", lineNumber);
            }
            
            var value = EvaluateExpression(valueExpr, lineNumber);
            _variables[constName] = value;
            _constants.Add(constName);
            return;
        }

        // Assignment with ← or <-
        var assignMatch = Regex.Match(line, @"^(\w+)\s*(←|<-)\s*(.+)$");
        if (assignMatch.Success)
        {
            var varName = assignMatch.Groups[1].Value;
            var expression = assignMatch.Groups[3].Value.Trim();
            
            if (!_variables.ContainsKey(varName))
            {
                throw new RuntimeException($"Variable '{varName}' has not been declared", lineNumber);
            }
            
            if (_constants.Contains(varName))
            {
                throw new RuntimeException($"Cannot reassign CONSTANT '{varName}'", lineNumber);
            }
            
            var value = EvaluateExpression(expression, lineNumber);
            _variables[varName] = value;
            return;
        }

        // If we get here, the statement is not recognized
        // For now, just ignore unrecognized statements (could be control flow keywords we haven't implemented yet)
    }

    private object EvaluateExpression(string expression, int lineNumber)
    {
        expression = expression.Trim();

        // String literal
        if (expression.StartsWith("\"") && expression.EndsWith("\""))
        {
            return expression[1..^1]; // Remove quotes
        }

        // Number literal
        if (double.TryParse(expression, out var number))
        {
            // Return integer if no decimal part
            if (number == Math.Floor(number))
            {
                return (int)number;
            }
            return number;
        }

        // Boolean literal
        if (expression.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        if (expression.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Arithmetic operations
        // Handle division first to detect division by zero
        var divMatch = Regex.Match(expression, @"^(.+?)\s*/\s*(.+)$");
        if (divMatch.Success)
        {
            var left = EvaluateExpression(divMatch.Groups[1].Value, lineNumber);
            var right = EvaluateExpression(divMatch.Groups[2].Value, lineNumber);
            
            var leftNum = Convert.ToDouble(left);
            var rightNum = Convert.ToDouble(right);
            
            if (Math.Abs(rightNum) < 0.0000001)
            {
                throw new RuntimeException("Cannot divide by zero", lineNumber);
            }
            
            return leftNum / rightNum;
        }

        // DIV operator (integer division)
        var divIntMatch = Regex.Match(expression, @"^(.+?)\s+DIV\s+(.+)$", RegexOptions.IgnoreCase);
        if (divIntMatch.Success)
        {
            var left = EvaluateExpression(divIntMatch.Groups[1].Value, lineNumber);
            var right = EvaluateExpression(divIntMatch.Groups[2].Value, lineNumber);
            
            var leftNum = Convert.ToInt32(left);
            var rightNum = Convert.ToInt32(right);
            
            if (rightNum == 0)
            {
                throw new RuntimeException("Cannot divide by zero", lineNumber);
            }
            
            return leftNum / rightNum;
        }

        // MOD operator
        var modMatch = Regex.Match(expression, @"^(.+?)\s+MOD\s+(.+)$", RegexOptions.IgnoreCase);
        if (modMatch.Success)
        {
            var left = EvaluateExpression(modMatch.Groups[1].Value, lineNumber);
            var right = EvaluateExpression(modMatch.Groups[2].Value, lineNumber);
            
            var leftNum = Convert.ToInt32(left);
            var rightNum = Convert.ToInt32(right);
            
            if (rightNum == 0)
            {
                throw new RuntimeException("Cannot perform MOD with zero", lineNumber);
            }
            
            return leftNum % rightNum;
        }

        // Addition
        var addMatch = Regex.Match(expression, @"^(.+?)\s*\+\s*(.+)$");
        if (addMatch.Success)
        {
            var left = EvaluateExpression(addMatch.Groups[1].Value, lineNumber);
            var right = EvaluateExpression(addMatch.Groups[2].Value, lineNumber);
            
            // String concatenation
            if (left is string || right is string)
            {
                return left.ToString() + right.ToString();
            }
            
            // Numeric addition
            return Convert.ToDouble(left) + Convert.ToDouble(right);
        }

        // Subtraction
        var subMatch = Regex.Match(expression, @"^(.+?)\s*-\s*(.+)$");
        if (subMatch.Success)
        {
            var left = EvaluateExpression(subMatch.Groups[1].Value, lineNumber);
            var right = EvaluateExpression(subMatch.Groups[2].Value, lineNumber);
            
            return Convert.ToDouble(left) - Convert.ToDouble(right);
        }

        // Multiplication
        var mulMatch = Regex.Match(expression, @"^(.+?)\s*\*\s*(.+)$");
        if (mulMatch.Success)
        {
            var left = EvaluateExpression(mulMatch.Groups[1].Value, lineNumber);
            var right = EvaluateExpression(mulMatch.Groups[2].Value, lineNumber);
            
            return Convert.ToDouble(left) * Convert.ToDouble(right);
        }

        // Variable reference
        if (_variables.ContainsKey(expression))
        {
            return _variables[expression];
        }

        // If nothing matches, might be an undefined variable
        if (Regex.IsMatch(expression, @"^\w+$"))
        {
            throw new RuntimeException($"Variable '{expression}' has not been declared", lineNumber);
        }

        // Otherwise return as-is (might be part of a more complex expression)
        return expression;
    }
}

/// <summary>
/// Exception for runtime errors during pseudocode execution
/// </summary>
public class RuntimeException : Exception
{
    public int Line { get; }

    public RuntimeException(string message, int line) : base(message)
    {
        Line = line;
    }
}
