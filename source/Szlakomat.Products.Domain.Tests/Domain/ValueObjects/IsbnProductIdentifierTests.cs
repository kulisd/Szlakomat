using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Tests.Domain.ValueObjects;

public class IsbnProductIdentifierTests
{
    [Theory]
    [InlineData("0201770601")] // valid ISBN-10 (K&R C)
    [InlineData("0306406152")] // valid ISBN-10
    [InlineData("080442957X")] // valid ISBN-10 with X check digit
    public void Create_WithValidIsbn_ShouldSucceed(string isbn)
    {
        // Act
        var identifier = IsbnProductIdentifier.Of(isbn);

        // Assert
        identifier.Value.Should().Be(isbn);
    }

    [Theory]
    [InlineData("0201770602")] // wrong check digit
    [InlineData("0306406151")] // wrong check digit
    public void Create_WithInvalidCheckDigit_ShouldThrowArgumentException(string isbn)
    {
        // Act
        var act = () => IsbnProductIdentifier.Of(isbn);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")] // empty
    [InlineData("   ")] // whitespace
    [InlineData("123")] // too short
    [InlineData("12345678901")] // too long
    [InlineData("abcdefghij")] // non-numeric
    public void Create_WithInvalidFormat_ShouldThrow(string isbn)
    {
        // Act
        var act = () => IsbnProductIdentifier.Of(isbn);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var act = () => new IsbnProductIdentifier(null);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithHyphens_ShouldNormalize()
    {
        // Act
        var identifier = IsbnProductIdentifier.Of("0-201-77060-1");

        // Assert
        identifier.Value.Should().Be("0201770601");
    }

    [Fact]
    public void Type_ShouldReturnIsbn()
    {
        // Arrange
        var identifier = IsbnProductIdentifier.Of("0201770601");

        // Act
        var type = identifier.Type();

        // Assert
        type.Should().Be("ISBN");
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var a = IsbnProductIdentifier.Of("0201770601");
        var b = IsbnProductIdentifier.Of("0201770601");

        // Act
        var result = a.Equals(b);

        // Assert
        result.Should().BeTrue();
    }
}
