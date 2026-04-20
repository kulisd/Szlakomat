using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Domain.Tests.Domain.ValueObjects;

public class ImeiSerialNumberTests
{
    [Theory]
    [InlineData("490154203237518")] // valid IMEI (Luhn OK)
    [InlineData("352099001761481")] // another valid IMEI
    public void Create_WithValidImei_ShouldSucceed(string imei)
    {
        // Act
        var serial = ImeiSerialNumber.Of(imei);

        // Assert
        serial.Value.Should().Be(imei);
        serial.Type.Should().Be("IMEI");
    }

    [Theory]
    [InlineData("490154203237510")] // wrong check digit
    [InlineData("000000000000001")] // Luhn fails
    public void Create_WithInvalidCheckDigit_ShouldThrowArgumentException(string imei)
    {
        // Act
        var act = () => ImeiSerialNumber.Of(imei);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")] // empty
    [InlineData("   ")] // whitespace
    [InlineData("12345")] // too short
    [InlineData("1234567890123456")] // too long (16 digits)
    [InlineData("49015420323751a")] // non-numeric
    public void Create_WithInvalidFormat_ShouldThrow(string imei)
    {
        // Act
        var act = () => ImeiSerialNumber.Of(imei);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var act = () => new ImeiSerialNumber(null);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithDashes_ShouldNormalize()
    {
        // Act
        var serial = ImeiSerialNumber.Of("49-0154-203237-518");

        // Assert
        serial.Value.Should().Be("490154203237518");
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var a = ImeiSerialNumber.Of("490154203237518");
        var b = ImeiSerialNumber.Of("490154203237518");

        // Act
        var result = a.Equals(b);

        // Assert
        result.Should().BeTrue();
    }
}
