using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using PseudocodeEditorAPI.Services;
using PseudocodeEditorAPI.Tests.TestData;

namespace PseudocodeEditorAPI.Tests.Services;

/// <summary>
/// Grey box tests for PseudocodeFormattingService
/// Tests internal formatting algorithms against Cambridge International standards
/// Validates keyword capitalization, indentation logic, and structure preservation
/// </summary>
public class PseudocodeFormattingServiceTests
{
    private readonly PseudocodeFormattingService _formattingService;

    public PseudocodeFormattingServiceTests()
    {
        _formattingService = new PseudocodeFormattingService();
    }

    #region Keyword Capitalization Tests

    [Fact]
    public async Task FormatAsync_WithLowercaseKeywords_ShouldCapitalize()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.LowercaseKeywords;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("PROCEDURE");
        result.Should().Contain("BYREF");
        result.Should().Contain("DECLARE");
        result.Should().Contain("ENDPROCEDURE");
        result.Should().NotContain("procedure");
        result.Should().NotContain("declare");
    }

    [Theory]
    [InlineData("if x > 0 then", "IF", "THEN")]
    [InlineData("while count < 10 do", "WHILE", "DO")]
    [InlineData("for i ← 1 to 10", "FOR", "TO")]
    [InlineData("repeat until x = 0", "REPEAT", "UNTIL")]
    [InlineData("case x of", "CASE", "OF")]
    public async Task FormatAsync_WithSpecificLowercaseKeywords_ShouldCapitalizeAll(
        string input, params string[] expectedKeywords)
    {
        // Act
        var result = await _formattingService.FormatAsync(input);

        // Assert
        foreach (var keyword in expectedKeywords)
        {
            result.Should().Contain(keyword);
        }
    }

    [Fact]
    public async Task FormatAsync_WithMixedCaseKeywords_ShouldNormalizeToUpperCase()
    {
        // Arrange
        var content = @"If Score > 100 Then
   Output ""High""
EndIf";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("IF");
        result.Should().Contain("THEN");
        result.Should().Contain("OUTPUT");
        result.Should().Contain("ENDIF");
    }

    [Theory]
    [MemberData(nameof(GetAllCambridgeKeywords))]
    public async Task FormatAsync_WithEachKeyword_ShouldCapitalize(string keyword)
    {
        // Arrange
        var content = $"{keyword.ToLower()} test";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain(keyword.ToUpper());
    }

    public static IEnumerable<object[]> GetAllCambridgeKeywords()
    {
        foreach (var keyword in CambridgePseudocodeExamples.AllKeywords)
        {
            yield return new object[] { keyword };
        }
    }

    #endregion

    #region Indentation Tests

    [Fact]
    public async Task FormatAsync_WithNoIndentation_ShouldAddProperIndentation()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.UnformattedCode;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().StartWith("IF"); // No indentation
        lines[1].Should().StartWith("   "); // 3 spaces
        lines[2].Should().StartWith("ELSE"); // Back to no indentation
        lines[3].Should().StartWith("   "); // 3 spaces
        lines[4].Should().StartWith("ENDIF"); // Back to no indentation
    }

    [Fact]
    public async Task FormatAsync_WithInconsistentIndentation_ShouldNormalize()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InconsistentIndentation;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        // All DECLARE statements should have same indentation (3 spaces)
        lines.Where(l => l.Contains("DECLARE")).Should().AllSatisfy(line =>
        {
            var leadingSpaces = line.TakeWhile(c => c == ' ').Count();
            leadingSpaces.Should().Be(3);
        });
    }

    [Fact]
    public async Task FormatAsync_WithNestedStructures_ShouldIndentCorrectly()
    {
        // Arrange
        var content = @"IF A > 0 THEN
IF B > 0 THEN
OUTPUT ""Nested""
ENDIF
ENDIF";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" "); // IF - 0 spaces
        lines[1].Should().StartWith("   "); // IF nested - 3 spaces
        lines[2].Should().StartWith("      "); // OUTPUT - 6 spaces
        lines[3].Should().StartWith("   "); // ENDIF - 3 spaces
        lines[4].Should().NotStartWith(" "); // ENDIF - 0 spaces
    }

    [Fact]
    public async Task FormatAsync_WithForLoop_ShouldIndentBody()
    {
        // Arrange
        var content = @"FOR Index ← 1 TO 10
OUTPUT Index
NEXT Index";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" ");
        lines[1].Should().StartWith("   ");
        lines[2].Should().NotStartWith(" ");
    }

    [Fact]
    public async Task FormatAsync_WithWhileLoop_ShouldIndentBody()
    {
        // Arrange
        var content = @"WHILE Count < 10 DO
Count ← Count + 1
ENDWHILE";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" ");
        lines[1].Should().StartWith("   ");
        lines[2].Should().NotStartWith(" ");
    }

    [Fact]
    public async Task FormatAsync_WithRepeatUntil_ShouldIndentBody()
    {
        // Arrange
        var content = @"REPEAT
INPUT Answer
UNTIL Answer = ""quit""";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" ");
        lines[1].Should().StartWith("   ");
        lines[2].Should().NotStartWith(" ");
    }

    [Fact]
    public async Task FormatAsync_WithCaseStatement_ShouldIndentCases()
    {
        // Arrange
        var content = @"CASE Grade OF
""A"": OUTPUT ""Excellent""
""B"": OUTPUT ""Good""
OTHERWISE: OUTPUT ""Try harder""
ENDCASE";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" ");
        lines[1].Should().StartWith("   ");
        lines[2].Should().StartWith("   ");
        lines[3].Should().StartWith("   ");
        lines[4].Should().NotStartWith(" ");
    }

    [Fact]
    public async Task FormatAsync_WithProcedure_ShouldIndentBody()
    {
        // Arrange
        var content = @"PROCEDURE Test()
DECLARE X : INTEGER
X ← 5
ENDPROCEDURE";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" ");
        lines[1].Should().StartWith("   ");
        lines[2].Should().StartWith("   ");
        lines[3].Should().NotStartWith(" ");
    }

    [Fact]
    public async Task FormatAsync_WithFunction_ShouldIndentBody()
    {
        // Arrange
        var content = @"FUNCTION Calc(X : INTEGER) RETURNS INTEGER
DECLARE Result : INTEGER
Result ← X * 2
RETURN Result
ENDFUNCTION";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[0].Should().NotStartWith(" ");
        lines.Skip(1).Take(3).Should().AllSatisfy(line =>
            line.Should().StartWith("   "));
        lines[4].Should().NotStartWith(" ");
    }

    #endregion

    #region Comment Preservation Tests

    [Fact]
    public async Task FormatAsync_WithComments_ShouldPreserveComments()
    {
        // Arrange
        var content = @"// This is a comment
DECLARE X : INTEGER
// Another comment
X ← 5";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("// This is a comment");
        result.Should().Contain("// Another comment");
    }

    [Fact]
    public async Task FormatAsync_WithCommentsInNestedStructure_ShouldIndentComments()
    {
        // Arrange
        var content = @"IF X > 0 THEN
// This is indented
OUTPUT X
ENDIF";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[1].Should().StartWith("   //");
    }

    #endregion

    #region String Literal Preservation Tests

    [Fact]
    public async Task FormatAsync_WithKeywordsInStrings_ShouldNotFormatThem()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.StringLiteralsWithKeywords;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("\"Please type IF or ELSE\"");
        result.Should().Contain("\"WHILE condition DO something\"");
    }

    [Fact]
    public async Task FormatAsync_WithDoubleQuotedStrings_ShouldPreserve()
    {
        // Arrange
        var content = @"OUTPUT ""Hello, World!""
DECLARE Message : STRING
Message ← ""Test message""";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("\"Hello, World!\"");
        result.Should().Contain("\"Test message\"");
    }

    [Fact]
    public async Task FormatAsync_WithSingleQuotedChars_ShouldPreserve()
    {
        // Arrange
        var content = @"DECLARE Ch : CHAR
Ch ← 'X'
OUTPUT Ch";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("'X'");
    }

    [Fact]
    public async Task FormatAsync_WithSpecialCharactersInStrings_ShouldPreserve()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.SpecialCharactersInStrings;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("O'Brien");
    }

    #endregion

    #region Identifier Preservation Tests

    [Fact]
    public async Task FormatAsync_WithCamelCaseIdentifiers_ShouldPreserve()
    {
        // Arrange
        var content = @"DECLARE StudentName : STRING
DECLARE TotalScore : INTEGER
StudentName ← ""John""";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("StudentName");
        result.Should().Contain("TotalScore");
        result.Should().NotContain("studentname");
    }

    [Fact]
    public async Task FormatAsync_WithPascalCaseIdentifiers_ShouldPreserve()
    {
        // Arrange
        var content = @"PROCEDURE CalculateTotal()
DECLARE Total : REAL
ENDPROCEDURE";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("CalculateTotal");
        result.Should().Contain("Total");
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public async Task FormatAsync_WithEmptyContent_ShouldReturnEmpty()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.EmptyContent;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FormatAsync_WithOnlyWhitespace_ShouldReturnEmpty()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.OnlyWhitespace;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Trim().Should().BeEmpty();
    }

    [Fact]
    public async Task FormatAsync_WithBlankLines_ShouldPreserveBlankLines()
    {
        // Arrange
        var content = @"DECLARE X : INTEGER

DECLARE Y : INTEGER";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        lines[1].Should().BeEmpty();
    }

    [Fact]
    public async Task FormatAsync_WithMultipleBlankLines_ShouldPreserve()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.MultipleBlankLines;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("\n\n");
    }

    [Fact]
    public async Task FormatAsync_WithSingleLine_ShouldFormat()
    {
        // Arrange
        var content = "declare x : integer";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Be("DECLARE x : INTEGER");
    }

    [Fact]
    public async Task FormatAsync_WithDeepNesting_ShouldHandleAllLevels()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.DeepNesting;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        var lines = result.Split('\n');
        
        // Check progressive indentation
        var outputLine = lines.First(l => l.Contains("Very deep"));
        var leadingSpaces = outputLine.TakeWhile(c => c == ' ').Count();
        leadingSpaces.Should().Be(15); // 5 levels * 3 spaces
    }

    #endregion

    #region Complex Real-World Examples Tests

    [Fact]
    public async Task FormatAsync_WithValidProcedureExample_ShouldFormatCorrectly()
    {
        // Arrange - Already well-formatted
        var content = CambridgePseudocodeExamples.ValidProcedureExample;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("PROCEDURE");
        result.Should().Contain("ENDPROCEDURE");
        var lines = result.Split('\n');
        lines[0].Should().StartWith("//");
        lines[2].Should().NotStartWith(" ");
    }

    [Fact]
    public async Task FormatAsync_WithComplexNestedStructure_ShouldFormatCorrectly()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ComplexNestedStructure;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("PROCEDURE");
        result.Should().Contain("FOR");
        result.Should().Contain("IF");
        result.Should().Contain("CASE");
        
        // Verify structure is maintained
        var lines = result.Split('\n');
        lines.Should().Contain(l => l.Trim().StartsWith("FOR"));
        lines.Should().Contain(l => l.Trim().StartsWith("IF"));
        lines.Should().Contain(l => l.Trim().StartsWith("CASE"));
    }

    [Fact]
    public async Task FormatAsync_WithUserDefinedType_ShouldFormatCorrectly()
    {
        // Arrange
        var content = @"type studentrecord
declare lastname : string
declare firstname : string
endtype";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("TYPE");
        result.Should().Contain("DECLARE");
        result.Should().Contain("ENDTYPE");
        
        var lines = result.Split('\n');
        lines.Where(l => l.Contains("DECLARE")).Should().AllSatisfy(line =>
            line.Should().StartWith("   "));
    }

    [Fact]
    public async Task FormatAsync_WithClassDefinition_ShouldFormatCorrectly()
    {
        // Arrange
        var content = @"class vehicle
private regnumber : string
public procedure new(regnum : string)
regnumber ← regnum
endprocedure
endclass";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("CLASS");
        result.Should().Contain("PRIVATE");
        result.Should().Contain("PUBLIC");
        result.Should().Contain("PROCEDURE");
        result.Should().Contain("ENDCLASS");
    }

    #endregion

    #region Operators and Symbols Tests

    [Fact]
    public async Task FormatAsync_WithAssignmentOperator_ShouldPreserve()
    {
        // Arrange
        var content = @"X ← 5
Y ← X + 10";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("←");
    }

    [Fact]
    public async Task FormatAsync_WithComparisonOperators_ShouldPreserve()
    {
        // Arrange
        var content = @"IF X > 5 THEN
   OUTPUT X
ENDIF
IF Y < 10 THEN
   OUTPUT Y
ENDIF";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain(">");
        result.Should().Contain("<");
    }

    [Fact]
    public async Task FormatAsync_WithArithmeticOperators_ShouldPreserve()
    {
        // Arrange
        var content = @"Result ← X + Y * Z / 2 - 1";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("+");
        result.Should().Contain("*");
        result.Should().Contain("/");
        result.Should().Contain("-");
    }

    [Fact]
    public async Task FormatAsync_WithModAndDiv_ShouldCapitalize()
    {
        // Arrange
        var content = @"Result ← X mod 10
Quotient ← Y div 3";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("MOD");
        result.Should().Contain("DIV");
    }

    [Fact]
    public async Task FormatAsync_WithLogicalOperators_ShouldCapitalize()
    {
        // Arrange
        var content = @"IF X > 0 and Y < 10 or not Z THEN
   OUTPUT ""Valid""
ENDIF";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("AND");
        result.Should().Contain("OR");
        result.Should().Contain("NOT");
    }

    #endregion

    #region Array and Data Structure Tests

    [Fact]
    public async Task FormatAsync_WithArrayDeclaration_ShouldFormat()
    {
        // Arrange
        var content = @"declare studentnames : array[1:30] of string";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("DECLARE");
        result.Should().Contain("ARRAY");
        result.Should().Contain("OF");
        result.Should().Contain("STRING");
        result.Should().Contain("[1:30]");
    }

    [Fact]
    public async Task FormatAsync_WithArrayAccess_ShouldPreserveStructure()
    {
        // Arrange
        var content = @"StudentNames[1] ← ""Alice""
OUTPUT StudentNames[Index]";

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().Contain("[1]");
        result.Should().Contain("[Index]");
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task FormatAsync_WithLargeContent_ShouldComplete()
    {
        // Arrange
        var lines = new List<string>();
        for (int i = 0; i < 1000; i++)
        {
            lines.Add($"declare var{i} : integer");
        }
        var content = string.Join("\n", lines);

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("DECLARE");
        result.Split('\n').Should().HaveCount(1000);
    }

    #endregion

    #region Idempotency Tests

    [Fact]
    public async Task FormatAsync_AppliedTwice_ShouldProduceSameResult()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.UnformattedCode;

        // Act
        var firstFormat = await _formattingService.FormatAsync(content);
        var secondFormat = await _formattingService.FormatAsync(firstFormat);

        // Assert
        secondFormat.Should().Be(firstFormat);
    }

    [Fact]
    public async Task FormatAsync_OnAlreadyFormattedCode_ShouldNotChange()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidProcedureExample;

        // Act
        var result = await _formattingService.FormatAsync(content);

        // Assert
        // Should be very similar (might differ in whitespace normalization)
        result.Should().Contain("PROCEDURE");
        result.Should().Contain("ENDPROCEDURE");
    }

    #endregion
}
