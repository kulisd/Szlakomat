using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Tests.Domain.ValueObjects;

public class GtinProductIdentifierTests
{
    [Theory]
    [InlineData("96385074")] // valid GTIN-8
    [InlineData("614141000036")] // valid GTIN-12 (UPC-A)
    [InlineData("4006381333931")] // valid GTIN-13 (EAN-13)
    [InlineData("00012345678905")] // valid GTIN-14
    public void Create_WithValidGtin_ShouldSucceed(string gtin)
    {
        // Act
        var identifier = GtinProductIdentifier.Of(gtin);

        // Assert
        identifier.Value.Should().Be(gtin);
    }

    [Theory]
    [InlineData("96385070")] // invalid check digit GTIN-8
    [InlineData("614141000030")] // invalid check digit GTIN-12
    [InlineData("4006381333930")] // invalid check digit GTIN-13
    public void Create_WithInvalidCheckDigit_ShouldThrowArgumentException(string gtin)
    {
        // Act
        var act = () => GtinProductIdentifier.Of(gtin);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")] // empty
    [InlineData("   ")] // whitespace
    [InlineData("1234")] // too short
    [InlineData("123456789")] // 9 digits - not valid length
    [InlineData("123456789012345")] // 15 digits - not valid length
    [InlineData("abcdefgh")] // non-numeric
    public void Create_WithInvalidFormat_ShouldThrow(string gtin)
    {
        // Act
        var act = () => GtinProductIdentifier.Of(gtin);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var act = () => new GtinProductIdentifier(null);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithHyphens_ShouldNormalize()
    {
        // Act
        var identifier = GtinProductIdentifier.Of("9638-5074");

        // Assert
        identifier.Value.Should().Be("96385074");
    }

    [Theory]
    [InlineData("96385074", "GTIN-8")]
    [InlineData("614141000036", "GTIN-12")]
    [InlineData("4006381333931", "GTIN-13")]
    [InlineData("00012345678905", "GTIN-14")]
    public void Type_ShouldReturnCorrectFormat(string gtin, string expectedType)
    {
        // Arrange
        var identifier = GtinProductIdentifier.Of(gtin);

        // Act
        var type = identifier.Type();

        // Assert
        type.Should().Be(expectedType);
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var a = GtinProductIdentifier.Of("96385074");
        var b = GtinProductIdentifier.Of("96385074");

        // Act
        var result = a.Equals(b);

        // Assert
        result.Should().BeTrue();
    }
}
