# Pseudocode Editor API - Test Suite

## Overview
This test suite uses a **Grey Box Testing** approach to validate the Pseudocode Editor API against Cambridge International AS & A Level Computer Science 9618 pseudocode standards.

## Grey Box Testing Approach

Grey box testing combines knowledge of internal implementation with external behavior validation:

### What We Test
1. **Internal Logic**: Validation algorithms, formatting rules, data processing workflows
2. **External Behavior**: API contracts, CRUD operations, error handling
3. **Integration Points**: Service orchestration, dependency interactions

### Why Grey Box?
- **Knowledge of Cambridge Standards**: We understand the pseudocode requirements (keywords, indentation, syntax rules)
- **Implementation Awareness**: We know the services use regex patterns, keyword sets, and indentation counters
- **Requirement Coverage**: Tests verify both that requirements are met and that implementation correctly enforces them

## Test Structure

```
PseudocodeEditorAPI.Tests/
├── Services/
│   ├── PseudocodeValidationServiceTests.cs   # 60+ tests
│   ├── PseudocodeFormattingServiceTests.cs   # 70+ tests
│   └── PseudocodeServiceTests.cs             # 40+ tests
├── TestData/
│   └── CambridgePseudocodeExamples.cs        # Cambridge standard examples
└── PseudocodeEditorAPI.Tests.csproj          # Test project configuration
```

## Test Categories

### 1. Validation Service Tests (`PseudocodeValidationServiceTests.cs`)

#### **Keyword Case Validation**
- Tests that keywords (IF, THEN, WHILE, etc.) must be uppercase
- Validates all 40+ Cambridge keywords
- Theory-based tests for common patterns

#### **Identifier Naming**
- Tests camelCase/PascalCase enforcement
- Detects snake_case usage
- Validates identifier conventions

#### **Indentation Rules**
- Tests 3-space indentation requirement
- Validates nested structure indentation
- Boundary tests for different indentation levels

#### **Syntax Error Detection**
- Unmatched parentheses detection
- Unmatched brackets detection
- Missing THEN after IF statements
- Line number accuracy

#### **Edge Cases**
- Empty content handling
- Comment-only files
- Keywords in string literals
- Deep nesting (5+ levels)
- Very long content (1000+ lines)

**Test Count**: 60+ tests covering all validation rules

### 2. Formatting Service Tests (`PseudocodeFormattingServiceTests.cs`)

#### **Keyword Capitalization**
- Auto-capitalizes all lowercase keywords
- Handles mixed-case keywords
- Theory tests for each Cambridge keyword

#### **Indentation Formatting**
- Applies 3-space indentation
- Handles nested structures (IF, FOR, WHILE, PROCEDURE, CLASS)
- Normalizes inconsistent indentation

#### **Structure Preservation**
- Preserves comments and positions them correctly
- Preserves string literals (keywords in strings not formatted)
- Preserves identifiers (camelCase maintained)
- Preserves operators and symbols

#### **Complex Structures**
- Procedures and functions
- User-defined types
- Class definitions
- Deeply nested control structures

#### **Idempotency**
- Formatting twice produces same result
- Already formatted code unchanged

**Test Count**: 70+ tests covering all formatting scenarios

### 3. Integration Service Tests (`PseudocodeServiceTests.cs`)

#### **CRUD Operations** (Grey Box Focus)
- Create: Validates orchestration with validation + formatting
- Read: Tests document retrieval by ID and bulk operations
- Update: Validates re-processing workflow
- Delete: Tests removal and cascade effects

#### **Service Orchestration**
- Mocked dependencies (validation & formatting services)
- Call order verification (validate before format)
- Integration workflow testing

#### **Business Logic**
- Empty title defaults to "Untitled"
- Whitespace trimming
- Unique ID generation
- Timestamp management

#### **Edge Cases**
- Empty/null content handling
- Non-existent document operations
- Multiple simultaneous documents

**Test Count**: 40+ tests covering orchestration and business logic

## Cambridge Requirements Coverage

### Covered Requirements

✅ **Font and Style** (Section 1.1)
- Monospaced font preservation
- Consistent presentation

✅ **Indentation** (Section 1.2)
- 3-space indentation requirement
- Nested structure indentation
- Continuation line handling

✅ **Case Rules** (Section 1.3)
- Keywords in UPPER CASE
- Identifiers in camelCase/PascalCase
- Data types validation

✅ **Comments** (Section 1.5)
- `//` comment syntax
- Multi-line comments
- Comment positioning

✅ **Data Types** (Section 2.1)
- INTEGER, REAL, CHAR, STRING, BOOLEAN, DATE

✅ **Keywords** (All Sections)
- Control structures: IF, WHILE, FOR, REPEAT, CASE
- Procedures/Functions: PROCEDURE, FUNCTION, RETURN
- Declarations: DECLARE, CONSTANT, ARRAY, TYPE
- File handling: OPENFILE, READFILE, WRITEFILE
- OOP: CLASS, PRIVATE, PUBLIC, INHERITS
- Operators: AND, OR, NOT, MOD, DIV

### Test Data (`CambridgePseudocodeExamples.cs`)

Contains authentic Cambridge examples:
- Valid procedure with BYREF parameters
- Variable and constant declarations
- Array declarations (1D and 2D)
- All control structures
- User-defined types
- Class definitions
- Invalid examples for error testing

## Testing Framework

### Technologies
- **xUnit**: Test framework (chosen for .NET 9 compatibility)
- **FluentAssertions**: Readable assertion library
- **Moq**: Mocking framework for grey box integration tests

### Test Patterns

#### 1. Arrange-Act-Assert (AAA)
```csharp
[Fact]
public async Task ValidateAsync_WithValidCode_ShouldReturnValid()
{
    // Arrange
    var content = CambridgePseudocodeExamples.ValidIfStatement;
    
    // Act
    var result = await _validationService.ValidateAsync(content);
    
    // Assert
    result.IsValid.Should().BeTrue();
}
```

#### 2. Theory-Based Testing
```csharp
[Theory]
[InlineData("if", "IF")]
[InlineData("while", "WHILE")]
public async Task FormatAsync_WithKeyword_ShouldCapitalize(string input, string expected)
{
    var result = await _formattingService.FormatAsync(input);
    result.Should().Contain(expected);
}
```

#### 3. Member Data Testing
```csharp
[Theory]
[MemberData(nameof(GetAllCambridgeKeywords))]
public async Task ValidateAsync_WithKeyword_ShouldRecognize(string keyword)
{
    // Test each Cambridge keyword
}
```

#### 4. Mock-Based Grey Box Testing
```csharp
[Fact]
public async Task CreateDocumentAsync_ShouldProcessInCorrectOrder()
{
    var callOrder = new List<string>();
    
    _mockValidationService
        .Setup(v => v.ValidateAsync(It.IsAny<string>()))
        .Callback(() => callOrder.Add("Validate"));
        
    _mockFormattingService
        .Setup(f => f.FormatAsync(It.IsAny<string>()))
        .Callback(() => callOrder.Add("Format"));
        
    await _service.CreateDocumentAsync(request);
    
    callOrder.Should().ContainInOrder("Validate", "Format");
}
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~PseudocodeValidationServiceTests"
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Tests in Watch Mode
```bash
dotnet watch test
```

## Test Coverage Goals

| Component | Target Coverage | Focus Areas |
|-----------|----------------|-------------|
| Validation Service | 90%+ | All validation rules, edge cases |
| Formatting Service | 90%+ | All formatting rules, idempotency |
| Main Service | 85%+ | CRUD operations, orchestration |
| Models | 100% | Simple DTOs and data classes |

## Key Testing Principles

### 1. **Requirements Traceability**
Every test traces back to a specific Cambridge requirement:
- Test names reference requirement sections
- Test data uses official examples
- Comments link to specification sections

### 2. **Boundary Value Analysis**
- 0, 3, 6, 9 spaces (valid indentation)
- 1, 2, 4, 5 spaces (invalid indentation)
- Empty, single line, 1000+ lines

### 3. **Equivalence Partitioning**
- Valid keywords vs invalid keywords
- Proper identifiers vs snake_case
- Matched vs unmatched brackets

### 4. **Error Path Testing**
- Validation errors (unmatched parens)
- Missing elements (no THEN after IF)
- Edge cases (empty content, whitespace)

### 5. **Integration Testing**
- Service orchestration
- Dependency interaction
- Workflow verification

## Grey Box Insights Applied

### Internal Knowledge Used:
1. **Keyword Set**: We know services use `HashSet<string>` for O(1) lookup
2. **Regex Patterns**: We test boundary cases that stress regex matching
3. **Indentation Counter**: We test indent level tracking with deep nesting
4. **Line-by-Line Processing**: We test line number reporting accuracy

### External Behavior Verified:
1. **API Contract**: Request/response models match specification
2. **Error Messages**: User-friendly, actionable error messages
3. **Performance**: Large file handling (1000+ lines)
4. **Correctness**: Output matches Cambridge standards

## Continuous Improvement

### Future Test Additions:
- [ ] Controller integration tests
- [ ] End-to-end API tests
- [ ] Performance benchmarks
- [ ] Mutation testing for validation rules
- [ ] Property-based testing for formatting idempotency

### Metrics to Track:
- Code coverage percentage
- Test execution time
- Number of requirements covered
- False positive/negative rates

## Contributing

When adding new tests:
1. Follow AAA pattern
2. Use descriptive test names
3. Add to appropriate category
4. Reference Cambridge specification
5. Include both positive and negative cases
6. Consider edge cases and boundaries

## References

- Cambridge International AS & A Level Computer Science 9618 Pseudocode Guide
- Grey Box Testing Methodology
- xUnit Documentation: https://xunit.net/
- FluentAssertions: https://fluentassertions.com/
- Moq: https://github.com/moq/moq4
