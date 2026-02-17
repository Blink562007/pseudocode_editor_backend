using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using PseudocodeEditorAPI.Models;
using PseudocodeEditorAPI.Data.Repositories;
using PseudocodeEditorAPI.Services;
using PseudocodeEditorAPI.Tests.TestData;

namespace PseudocodeEditorAPI.Tests.Services;

/// <summary>
/// Grey box integration tests for PseudocodeService
/// Tests orchestration logic with mocked dependencies
/// Validates CRUD operations, validation integration, and formatting workflows
/// </summary>
public class PseudocodeService_CreateDocumentTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_CreateDocumentTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Create Document Tests

    [Fact]
    public async Task CreateDocumentAsync_WithValidContent_ShouldCreateDocument()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test Document",
            Content = CambridgePseudocodeExamples.ValidIfStatement,
            Language = "pseudocode"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Title.Should().Be("Test Document");
        result.Content.Should().Be(request.Content);
        result.Language.Should().Be("pseudocode");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateDocumentAsync_ShouldCallValidationService()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "DECLARE X : INTEGER"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        await _service.CreateDocumentAsync(request);

        // Assert
        _mockValidationService.Verify(v => v.ValidateAsync(request.Content), Times.Once);
    }

    [Fact]
    public async Task CreateDocumentAsync_ShouldCallFormattingService()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "declare x : integer"
        };

        var formattedContent = "DECLARE X : INTEGER";

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(request.Content))
            .ReturnsAsync(formattedContent);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        _mockFormattingService.Verify(f => f.FormatAsync(request.Content), Times.Once);
        result.Content.Should().Be(formattedContent);
    }

    [Fact]
    public async Task CreateDocumentAsync_WithEmptyTitle_ShouldUseDefaultTitle()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "",
            Content = "DECLARE X : INTEGER"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Title.Should().Be("Untitled");
    }

    [Fact]
    public async Task CreateDocumentAsync_WithWhitespaceTitle_ShouldUseDefaultTitle()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "   ",
            Content = "DECLARE X : INTEGER"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Title.Should().Be("Untitled");
    }

    [Fact]
    public async Task CreateDocumentAsync_WithTitleWithWhitespace_ShouldTrim()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "  My Document  ",
            Content = "DECLARE X : INTEGER"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Title.Should().Be("My Document");
    }

    [Fact]
    public async Task CreateDocumentAsync_WithEmptyContent_ShouldReturnEmptyContent()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Empty Doc",
            Content = ""
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Content.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateDocumentAsync_ShouldGenerateUniqueIds()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "DECLARE X : INTEGER"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var doc1 = await _service.CreateDocumentAsync(request);
        var doc2 = await _service.CreateDocumentAsync(request);

        // Assert
        doc1.Id.Should().NotBe(doc2.Id);
    }

    #endregion
}

public class PseudocodeService_GetDocumentTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_GetDocumentTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Get Document Tests

    [Fact]
    public async Task GetDocumentByIdAsync_WithExistingId_ShouldReturnDocument()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "DECLARE X : INTEGER"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        var created = await _service.CreateDocumentAsync(request);

        // Act
        var result = await _service.GetDocumentByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Title.Should().Be(created.Title);
    }

    [Fact]
    public async Task GetDocumentByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetDocumentByIdAsync("nonexistent-id");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllDocumentsAsync_WithMultipleDocuments_ShouldReturnAll()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        await _service.CreateDocumentAsync(new CreatePseudocodeRequest { Title = "Doc1", Content = "X ← 1" });
        await _service.CreateDocumentAsync(new CreatePseudocodeRequest { Title = "Doc2", Content = "Y ← 2" });
        await _service.CreateDocumentAsync(new CreatePseudocodeRequest { Title = "Doc3", Content = "Z ← 3" });

        // Act
        var result = await _service.GetAllDocumentsAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllDocumentsAsync_WithNoDocuments_ShouldReturnEmpty()
    {
        // Act
        var result = await _service.GetAllDocumentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion
}

public class PseudocodeService_UpdateDocumentTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_UpdateDocumentTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Update Document Tests

    [Fact]
    public async Task UpdateDocumentAsync_WithExistingId_ShouldUpdateDocument()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Original Title",
            Content = "DECLARE X : INTEGER"
        });

        var updateRequest = new UpdatePseudocodeRequest
        {
            Title = "Updated Title",
            Content = "DECLARE Y : REAL"
        };

        // Act
        var result = await _service.UpdateDocumentAsync(created.Id, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Title.Should().Be("Updated Title");
        result.Content.Should().Be("DECLARE Y : REAL");
        result.UpdatedAt.Should().BeAfter(created.CreatedAt);
    }

    [Fact]
    public async Task UpdateDocumentAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var updateRequest = new UpdatePseudocodeRequest
        {
            Title = "Test",
            Content = "DECLARE X : INTEGER"
        };

        // Act
        var result = await _service.UpdateDocumentAsync("nonexistent-id", updateRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateDocumentAsync_ShouldCallValidationService()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1"
        });

        var updateRequest = new UpdatePseudocodeRequest
        {
            Title = "Updated",
            Content = "Y ← 2"
        };

        // Reset mock to clear previous calls
        _mockValidationService.Invocations.Clear();

        // Act
        await _service.UpdateDocumentAsync(created.Id, updateRequest);

        // Assert
        _mockValidationService.Verify(v => v.ValidateAsync(updateRequest.Content), Times.Once);
    }

    [Fact]
    public async Task UpdateDocumentAsync_ShouldCallFormattingService()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content.ToUpper());

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1"
        });

        var updateRequest = new UpdatePseudocodeRequest
        {
            Title = "Updated",
            Content = "declare y : integer"
        };

        // Reset mock to clear previous calls
        _mockFormattingService.Invocations.Clear();

        // Act
        await _service.UpdateDocumentAsync(created.Id, updateRequest);

        // Assert
        _mockFormattingService.Verify(f => f.FormatAsync(updateRequest.Content), Times.Once);
    }

    [Fact]
    public async Task UpdateDocumentAsync_WithEmptyTitle_ShouldKeepOriginalTitle()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Original Title",
            Content = "X ← 1"
        });

        var updateRequest = new UpdatePseudocodeRequest
        {
            Title = "",
            Content = "Y ← 2"
        };

        // Act
        var result = await _service.UpdateDocumentAsync(created.Id, updateRequest);

        // Assert
        result!.Title.Should().Be("Original Title");
    }

    [Fact]
    public async Task UpdateDocumentAsync_ShouldUpdateTimestamp()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1"
        });

        var originalUpdatedAt = created.UpdatedAt;
        
        // Wait a bit to ensure different timestamp
        await Task.Delay(10);

        var updateRequest = new UpdatePseudocodeRequest
        {
            Title = "Updated",
            Content = "Y ← 2"
        };

        // Act
        var result = await _service.UpdateDocumentAsync(created.Id, updateRequest);

        // Assert
        result!.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    #endregion
}

public class PseudocodeService_DeleteDocumentTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_DeleteDocumentTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Delete Document Tests

    [Fact]
    public async Task DeleteDocumentAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1"
        });

        // Act
        var result = await _service.DeleteDocumentAsync(created.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDocumentAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteDocumentAsync("nonexistent-id");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteDocumentAsync_ShouldRemoveDocument()
    {
        // Arrange
        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) => content);

        var created = await _service.CreateDocumentAsync(new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1"
        });

        // Act
        await _service.DeleteDocumentAsync(created.Id);
        var result = await _service.GetDocumentByIdAsync(created.Id);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}

public class PseudocodeService_ValidationIntegrationTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_ValidationIntegrationTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Validation Service Integration Tests

    [Fact]
    public async Task ValidateContentAsync_ShouldCallValidationService()
    {
        // Arrange
        var content = CambridgePseudocodeExamples.ValidIfStatement;
        var expectedResult = new ValidationResult { IsValid = true };

        _mockValidationService
            .Setup(v => v.ValidateAsync(content))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ValidateContentAsync(content);

        // Assert
        _mockValidationService.Verify(v => v.ValidateAsync(content), Times.Once);
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ValidateContentAsync_WithInvalidContent_ShouldReturnErrors()
    {
        // Arrange
        var content = "Result ← (5 + 3";
        var expectedResult = new ValidationResult
        {
            IsValid = false,
            Errors = new List<ValidationError>
            {
                new ValidationError { Code = "UNMATCHED_PARENS", Message = "Unmatched parentheses" }
            }
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(content))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ValidateContentAsync(content);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    #endregion
}

public class PseudocodeService_FormattingIntegrationTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_FormattingIntegrationTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Formatting Service Integration Tests

    [Fact]
    public async Task FormatContentAsync_ShouldCallFormattingService()
    {
        // Arrange
        var content = "declare x : integer";
        var expectedFormatted = "DECLARE X : INTEGER";

        _mockFormattingService
            .Setup(f => f.FormatAsync(content))
            .ReturnsAsync(expectedFormatted);

        // Act
        var result = await _service.FormatContentAsync(content);

        // Assert
        _mockFormattingService.Verify(f => f.FormatAsync(content), Times.Once);
        result.Should().Be(expectedFormatted);
    }

    #endregion
}

public class PseudocodeService_ContentProcessingWorkflowTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_ContentProcessingWorkflowTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Content Processing Workflow Tests

    [Fact]
    public async Task CreateDocumentAsync_ShouldProcessContentInCorrectOrder()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "declare x : integer"
        };

        var callOrder = new List<string>();

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .Callback(() => callOrder.Add("Validate"))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .Callback(() => callOrder.Add("Format"))
            .ReturnsAsync("DECLARE X : INTEGER");

        // Act
        await _service.CreateDocumentAsync(request);

        // Assert
        callOrder.Should().ContainInOrder("Validate", "Format");
    }

    [Fact]
    public async Task CreateDocumentAsync_WithComplexContent_ShouldProcessCorrectly()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Complex Example",
            Content = CambridgePseudocodeExamples.ComplexNestedStructure
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Content.Should().Contain("PROCEDURE");
        result.Content.Should().Contain("FOR");
        result.Content.Should().Contain("CASE");
    }

    #endregion
}

public class PseudocodeService_EdgeCasesTests
{
    private readonly Mock<IPseudocodeValidationService> _mockValidationService;
    private readonly Mock<IPseudocodeFormattingService> _mockFormattingService;
    private readonly IPseudocodeDocumentRepository _repository;
    private readonly PseudocodeService _service;

    public PseudocodeService_EdgeCasesTests()
    {
        _mockValidationService = new Mock<IPseudocodeValidationService>();
        _mockFormattingService = new Mock<IPseudocodeFormattingService>();
        _repository = new InMemoryPseudocodeDocumentRepository();
        _service = new PseudocodeService(_repository, _mockValidationService.Object, _mockFormattingService.Object);
    }

    #region Edge Cases

    [Fact]
    public async Task CreateDocumentAsync_WithNullLanguage_ShouldUseDefault()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1",
            Language = null!
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Language.Should().Be("pseudocode");
    }

    [Fact]
    public async Task CreateDocumentAsync_WithCustomLanguage_ShouldPreserve()
    {
        // Arrange
        var request = new CreatePseudocodeRequest
        {
            Title = "Test",
            Content = "X ← 1",
            Language = "cambridge-pseudocode"
        };

        _mockValidationService
            .Setup(v => v.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new ValidationResult { IsValid = true });

        _mockFormattingService
            .Setup(f => f.FormatAsync(It.IsAny<string>()))
            .ReturnsAsync(request.Content);

        // Act
        var result = await _service.CreateDocumentAsync(request);

        // Assert
        result.Language.Should().Be("cambridge-pseudocode");
    }

    #endregion
}
