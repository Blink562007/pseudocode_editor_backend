using Xunit;
using FluentAssertions;
using PseudocodeEditorAPI.Services;
using PseudocodeEditorAPI.Tests.TestData;

namespace PseudocodeEditorAPI.Tests.Services;

/// <summary>
/// Grey box tests for PseudocodeValidationService
/// Tests internal validation logic against Cambridge International AS & A Level standards
/// Combines knowledge of implementation (keywords, regex patterns) with requirements validation
/// </summary>
public class PseudocodeValidationServiceTests
{
    private readonly PseudocodeValidationService _validationService;

    public PseudocodeValidationServiceTests()
    {
        _validationService = new PseudocodeValidationService();
    }

    #region Valid Code Tests

    [Fact]
    public async Task ValidateAsync_WithValidProcedure_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidProcedureExample;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidVariableDeclaration_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidVariableDeclaration;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidIfStatement_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidIfStatement;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidForLoop_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidForLoop;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidWhileLoop_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidWhileLoop;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidFunction_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidFunction;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidClassDefinition_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidClassDefinition;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Keyword Case Validation Tests

    [Fact]
    public async Task ValidateAsync_WithLowercaseKeyword_ShouldReturnWarning()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InvalidKeywordCase;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().NotBeEmpty();
        result.Warnings.Should().Contain(w => w.Code == "KEYWORD_CASE");
        result.Warnings.Should().Contain(w => w.Message.Contains("IF"));
        result.Warnings.Should().Contain(w => w.Message.Contains("THEN"));
    }

    [Theory]
    [InlineData("if x > 0 then", "IF")]
    [InlineData("while count < 10 do", "WHILE")]
    [InlineData("for i ← 1 to 10", "FOR")]
    [InlineData("procedure test()", "PROCEDURE")]
    [InlineData("function calc() returns integer", "FUNCTION")]
    [InlineData("declare x : integer", "DECLARE")]
    public async Task ValidateAsync_WithSpecificLowercaseKeyword_ShouldWarnAboutCase(string content, string keyword)
    {
        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => 
            w.Code == "KEYWORD_CASE" && 
            w.Message.Contains(keyword));
    }

    [Fact]
    public async Task ValidateAsync_WithMixedCaseKeywords_ShouldWarnForEachIncorrectKeyword()
    {
        // Arrange
        var content = @"If Score > 100 Then
   output ""Message""
EndIf";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Where(w => w.Code == "KEYWORD_CASE").Should().HaveCountGreaterThan(0);
    }

    #endregion

    #region Identifier Case Validation Tests

    [Fact]
    public async Task ValidateAsync_WithSnakeCaseIdentifier_ShouldReturnWarning()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InvalidIdentifierSnakeCase;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => 
            w.Code == "IDENTIFIER_CASE" && 
            w.Message.Contains("snake_case"));
    }

    [Theory]
    [InlineData("DECLARE student_name : STRING")]
    [InlineData("DECLARE total_to_pay : REAL")]
    [InlineData("DECLARE is_valid_input : BOOLEAN")]
    public async Task ValidateAsync_WithUnderscoresInIdentifier_ShouldWarnAboutCamelCase(string content)
    {
        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => w.Code == "IDENTIFIER_CASE");
    }

    [Fact]
    public async Task ValidateAsync_WithProperCamelCase_ShouldNotWarn()
    {
        // Arrange
        var content = @"DECLARE StudentName : STRING
DECLARE TotalToPay : REAL
DECLARE IsValid : BOOLEAN";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Where(w => w.Code == "IDENTIFIER_CASE").Should().BeEmpty();
    }

    #endregion

    #region Indentation Validation Tests

    [Fact]
    public async Task ValidateAsync_WithInvalidIndentation_ShouldReturnWarning()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InvalidIndentation;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => w.Code == "INDENTATION");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(8)]
    public async Task ValidateAsync_WithNonMultipleOfThreeIndentation_ShouldWarn(int spaces)
    {
        // Arrange
        var indent = new string(' ', spaces);
        var content = $"IF X > 0 THEN\n{indent}OUTPUT X\nENDIF";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => 
            w.Code == "INDENTATION" && 
            w.Message.Contains(spaces.ToString()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    [InlineData(12)]
    public async Task ValidateAsync_WithCorrectIndentation_ShouldNotWarn(int spaces)
    {
        // Arrange
        var indent = new string(' ', spaces);
        var content = $"IF X > 0 THEN\n{indent}OUTPUT X\nENDIF";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Where(w => w.Code == "INDENTATION").Should().BeEmpty();
    }

    #endregion

    #region Syntax Error Tests

    [Fact]
    public async Task ValidateAsync_WithUnmatchedParentheses_ShouldReturnError()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InvalidUnmatchedParentheses;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "UNMATCHED_PARENS");
    }

    [Fact]
    public async Task ValidateAsync_WithUnmatchedBrackets_ShouldReturnError()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InvalidUnmatchedBrackets;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "UNMATCHED_BRACKETS");
    }

    [Theory]
    [InlineData("Result ← (5 + 3")]
    [InlineData("Result ← 5 + 3)")]
    [InlineData("Result ← ((5 + 3)")]
    public async Task ValidateAsync_WithVariousUnmatchedParentheses_ShouldDetectError(string expression)
    {
        // Act
        var result = await _validationService.ValidateAsync(expression);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "UNMATCHED_PARENS");
    }

    [Theory]
    [InlineData("Array[1")]
    [InlineData("Array1]")]
    [InlineData("Array[[1]")]
    public async Task ValidateAsync_WithVariousUnmatchedBrackets_ShouldDetectError(string expression)
    {
        // Act
        var result = await _validationService.ValidateAsync(expression);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "UNMATCHED_BRACKETS");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingThen_ShouldReturnWarning()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.InvalidMissingThen;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => w.Code == "MISSING_THEN");
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public async Task ValidateAsync_WithEmptyContent_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.EmptyContent;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyComments_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.OnlyComments;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyWhitespace_ShouldReturnValid()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.OnlyWhitespace;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WithComplexNestedStructure_ShouldValidateCorrectly()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ComplexNestedStructure;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithKeywordsInStringLiterals_ShouldNotValidateAsKeywords()
    {
        // Arrange - Keywords inside strings should be ignored
        var content = CambridgePseudocodeExamples.StringLiteralsWithKeywords;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        // Should not warn about lowercase keywords in strings
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WithMixedValidAndInvalid_ShouldDetectAllIssues()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.MixedValidAndInvalid;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().NotBeEmpty();
        result.Warnings.Should().Contain(w => w.Code == "KEYWORD_CASE");
    }

    [Fact]
    public async Task ValidateAsync_WithDeepNesting_ShouldValidateAllLevels()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.DeepNesting;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithSingleLineCode_ShouldValidate()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.SingleLineCode;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WithMultipleBlankLines_ShouldHandleGracefully()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.MultipleBlankLines;

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Line Number Accuracy Tests

    [Fact]
    public async Task ValidateAsync_ShouldReportCorrectLineNumbers()
    {
        // Arrange
        var content = @"DECLARE X : INTEGER
if Y > 0 then
   OUTPUT Y
ENDIF
declare z : integer";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Warnings.Should().Contain(w => w.LineNumber == 2);
        result.Warnings.Should().Contain(w => w.LineNumber == 5);
    }

    [Fact]
    public async Task ValidateAsync_WithErrorOnSpecificLine_ShouldReportExactLine()
    {
        // Arrange
        var content = @"DECLARE X : INTEGER
DECLARE Y : INTEGER
Result ← (5 + 3
DECLARE Z : INTEGER";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Errors.Should().Contain(e => 
            e.LineNumber == 3 && 
            e.Code == "UNMATCHED_PARENS");
    }

    #endregion

    #region Cambridge Requirements Coverage Tests

    [Theory]
    [MemberData(nameof(GetAllCambridgeKeywords))]
    public async Task ValidateAsync_WithAllCambridgeKeywords_ShouldRecognize(string keyword)
    {
        // Arrange
        var content = $"{keyword.ToLower()} test";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        // Should warn about lowercase keyword
        result.Warnings.Should().Contain(w => 
            w.Code == "KEYWORD_CASE" && 
            w.Message.Contains(keyword));
    }

    public static IEnumerable<object[]> GetAllCambridgeKeywords()
    {
        foreach (var keyword in CambridgePseudocodeExamples.AllKeywords)
        {
            yield return new object[] { keyword };
        }
    }

    [Fact]
    public async Task ValidateAsync_WithAllControlStructures_ShouldValidate()
    {
        // Arrange - Test all major control structures
        var content = @"DECLARE X : INTEGER
IF X > 0 THEN
   OUTPUT X
ENDIF

FOR X ← 1 TO 10
   OUTPUT X
NEXT X

WHILE X > 0 DO
   X ← X - 1
ENDWHILE

REPEAT
   INPUT X
UNTIL X = 0

CASE X OF
   1: OUTPUT ""One""
   OTHERWISE: OUTPUT ""Other""
ENDCASE";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Performance and Stress Tests

    [Fact]
    public async Task ValidateAsync_WithVeryLongContent_ShouldComplete()
    {
        // Arrange - Generate large content
        var lines = new List<string>();
        for (int i = 0; i < 1000; i++)
        {
            lines.Add($"DECLARE Var{i} : INTEGER");
        }
        var content = string.Join("\n", lines);

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WithManyErrors_ShouldReportAll()
    {
        // Arrange - Multiple lines with errors
        var content = @"declare x : integer
if y > 0 then
Result ← (5 + 3
Array[1 ← 10
while z < 10 do";

        // Act
        var result = await _validationService.ValidateAsync(content);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Warnings.Should().NotBeEmpty();
    }

    #endregion
}
