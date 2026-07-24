using System.ComponentModel.DataAnnotations;
using ExampleTemplate.WebApp.Validation;
using Shouldly;

namespace ExampleTemplate.WebApp.Tests.UnitTests.BuiltIn;

[TestFixture, Category("Unit"), Parallelizable(ParallelScope.All)]
public sealed class NotEmptyGuidTests
{
    [Test]
    public void IsValid_ValidNonEmptyGuid_ReturnsTrue()
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();
        var validGuid = Guid.NewGuid();

        // Act
        var result = attribute.IsValid(validGuid);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void IsValid_EmptyGuid_ReturnsFalse()
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();
        var emptyGuid = Guid.Empty;

        // Act
        var result = attribute.IsValid(emptyGuid);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void IsValid_NullValue_ReturnsFalse()
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();

        // Act
        var result = attribute.IsValid(null);

        // Assert
        result.ShouldBeFalse();
    }

    [TestCase("not a guid")]
    [TestCase(123)]
    [TestCase(true)]
    public void IsValid_NonGuidType_ReturnsFalse(object value)
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();

        // Act
        var result = attribute.IsValid(value);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void IsValid_WithValidationContext_ValidGuid_ReturnsSuccess()
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();
        var validGuid = Guid.NewGuid();
        var context = new ValidationContext(validGuid);

        // Act
        var result = attribute.GetValidationResult(validGuid, context);

        // Assert
        result.ShouldBe(ValidationResult.Success);
    }

    [Test]
    public void IsValid_WithValidationContext_EmptyGuid_ReturnsError()
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();
        var emptyGuid = Guid.Empty;
        var context = new ValidationContext(emptyGuid);

        // Act
        var result = attribute.GetValidationResult(emptyGuid, context);

        // Assert
        result.ShouldNotBeNull();
        result!.ErrorMessage.ShouldBe("UUID cannot be zero/default");
    }

    [Test]
    public void IsValid_WithValidationContext_NullValue_ReturnsError()
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        result.ShouldNotBeNull();
        result!.ErrorMessage.ShouldBe("UUID cannot be zero/default");
    }

    [TestCase("not a guid")]
    [TestCase(123)]
    [TestCase(true)]
    public void IsValid_WithValidationContext_NonGuidType_ReturnsError(object value)
    {
        // Arrange
        var attribute = new NotEmptyGuidAttribute();
        var context = new ValidationContext(value);

        // Act
        var result = attribute.GetValidationResult(value, context);

        // Assert
        result.ShouldNotBeNull();
        result!.ErrorMessage.ShouldBe("UUID cannot be zero/default");
    }
}