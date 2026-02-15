namespace PseudocodeEditorAPI.Tests.TestData;

/// <summary>
/// Test data based on Cambridge International AS & A Level pseudocode requirements
/// Provides valid and invalid examples for grey box testing
/// </summary>
public static class CambridgePseudocodeExamples
{
    #region Valid Examples from Cambridge Specification
    
    public const string ValidProcedureExample = @"// this procedure swaps
// values of X and Y
PROCEDURE SWAP(BYREF X : INTEGER, Y : INTEGER)
   Temp ← X
   X ← Y
   Y ← Temp
ENDPROCEDURE";

    public const string ValidVariableDeclaration = @"DECLARE Counter : INTEGER
DECLARE TotalToPay : REAL
DECLARE GameOver : BOOLEAN";

    public const string ValidConstantDeclaration = @"CONSTANT HourlyRate = 6.50
CONSTANT DefaultText = ""N/A""";

    public const string ValidArrayDeclaration = @"DECLARE StudentNames : ARRAY[1:30] OF STRING
DECLARE NoughtsAndCrosses : ARRAY[1:3,1:3] OF CHAR";

    public const string ValidIfStatement = @"IF Score > 100 THEN
   OUTPUT ""High score!""
ELSE
   OUTPUT ""Try again""
ENDIF";

    public const string ValidForLoop = @"FOR Index ← 1 TO 30
   StudentNames[Index] ← """"
NEXT Index";

    public const string ValidWhileLoop = @"WHILE Counter < 10 DO
   Counter ← Counter + 1
   OUTPUT Counter
ENDWHILE";

    public const string ValidRepeatLoop = @"REPEAT
   INPUT Answer
   Counter ← Counter + 1
UNTIL Answer = ""quit""";

    public const string ValidCaseStatement = @"CASE Grade OF
   ""A"": OUTPUT ""Excellent""
   ""B"": OUTPUT ""Good""
   OTHERWISE: OUTPUT ""Try harder""
ENDCASE";

    public const string ValidFunction = @"FUNCTION CalculateArea(Length : REAL, Width : REAL) RETURNS REAL
   DECLARE Area : REAL
   Area ← Length * Width
   RETURN Area
ENDFUNCTION";

    public const string ValidUserDefinedType = @"TYPE StudentRecord
   DECLARE LastName : STRING
   DECLARE FirstName : STRING
   DECLARE DateOfBirth : DATE
   DECLARE YearGroup : INTEGER
   DECLARE FormGroup : CHAR
ENDTYPE";

    public const string ValidClassDefinition = @"CLASS Vehicle
   PRIVATE RegNumber : STRING
   PRIVATE Speed : INTEGER
   
   PUBLIC PROCEDURE NEW(RegNum : STRING)
      RegNumber ← RegNum
      Speed ← 0
   ENDPROCEDURE
   
   PUBLIC PROCEDURE SetSpeed(NewSpeed : INTEGER)
      Speed ← NewSpeed
   ENDPROCEDURE
ENDCLASS";

    #endregion

    #region Invalid Examples - Testing Validation

    public const string InvalidKeywordCase = @"if Score > 100 then
   output ""High score!""
endif";

    public const string InvalidIdentifierSnakeCase = @"DECLARE total_to_pay : REAL
DECLARE student_name : STRING";

    public const string InvalidIndentation = @"IF Score > 100 THEN
  OUTPUT ""Wrong indentation""
ENDIF";

    public const string InvalidUnmatchedParentheses = @"DECLARE Result : REAL
Result ← (5 + 3 * 2";

    public const string InvalidUnmatchedBrackets = @"DECLARE Numbers : ARRAY[1:10] OF INTEGER
Numbers[5 ← 100";

    public const string InvalidMissingThen = @"IF Counter > 10
   Counter ← 0
ENDIF";

    #endregion

    #region Edge Cases

    public const string EmptyContent = "";

    public const string OnlyComments = @"// This is a comment
// Another comment
// Yet another comment";

    public const string OnlyWhitespace = "   \n\n   \n";

    public const string MixedValidAndInvalid = @"DECLARE Counter : INTEGER
if Counter > 10 then
   OUTPUT ""Mixed case issues""
ENDIF";

    public const string ComplexNestedStructure = @"PROCEDURE ProcessData()
   DECLARE Index : INTEGER
   DECLARE Total : REAL
   Total ← 0
   FOR Index ← 1 TO 100
      IF Data[Index] > 0 THEN
         CASE Data[Index] OF
            1: Total ← Total + 1.5
            2: Total ← Total + 2.5
            OTHERWISE: Total ← Total + Data[Index]
         ENDCASE
      ENDIF
   NEXT Index
   RETURN Total
ENDPROCEDURE";

    public const string StringLiteralsWithKeywords = @"OUTPUT ""Please type IF or ELSE""
DECLARE Message : STRING
Message ← ""WHILE condition DO something""";

    public const string SpecialCharactersInStrings = @"OUTPUT ""Name: O'Brien""
DECLARE Quote : STRING
Quote ← ""He said, \""Hello\""""";

    #endregion

    #region Formatting Test Cases

    public const string UnformattedCode = @"if score>100 then
output ""High score!""
else
output ""Try again""
endif";

    public const string InconsistentIndentation = @"PROCEDURE Test()
 DECLARE X : INTEGER
   DECLARE Y : INTEGER
  X ← 5
    Y ← 10
ENDPROCEDURE";

    public const string LowercaseKeywords = @"procedure swap(byref x : integer, y : integer)
   declare temp : integer
   temp ← x
   x ← y
   y ← temp
endprocedure";

    #endregion

    #region Boundary Testing

    public const string SingleLineCode = @"DECLARE X : INTEGER";

    public const string VeryLongLine = @"DECLARE VeryLongIdentifierNameThatExceedsNormalLength : STRING";

    public const string MultipleBlankLines = @"DECLARE X : INTEGER


DECLARE Y : INTEGER



DECLARE Z : INTEGER";

    public const string DeepNesting = @"IF A > 0 THEN
   IF B > 0 THEN
      IF C > 0 THEN
         IF D > 0 THEN
            IF E > 0 THEN
               OUTPUT ""Very deep""
            ENDIF
         ENDIF
      ENDIF
   ENDIF
ENDIF";

    #endregion

    #region Cambridge Specification Keywords

    public static readonly string[] AllKeywords = new[]
    {
        "IF", "THEN", "ELSE", "ENDIF",
        "CASE", "OF", "OTHERWISE", "ENDCASE",
        "FOR", "TO", "STEP", "NEXT",
        "REPEAT", "UNTIL",
        "WHILE", "DO", "ENDWHILE",
        "PROCEDURE", "ENDPROCEDURE",
        "FUNCTION", "RETURNS", "ENDFUNCTION",
        "DECLARE", "CONSTANT",
        "ARRAY", "TYPE", "ENDTYPE",
        "CALL", "BYREF", "BYVAL",
        "OUTPUT", "INPUT",
        "OPENFILE", "READFILE", "WRITEFILE", "CLOSEFILE", "EOF",
        "AND", "OR", "NOT", "MOD", "DIV",
        "CLASS", "ENDCLASS", "NEW", "PUBLIC", "PRIVATE", "INHERITS"
    };

    public static readonly string[] DataTypes = new[]
    {
        "INTEGER", "REAL", "CHAR", "STRING", "BOOLEAN", "DATE"
    };

    #endregion
}
