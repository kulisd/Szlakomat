using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Domain.Tests.Domain.ValueObjects;

public class NumericRangeConstraintTests
{
    [Fact]
    public void Create_WithValidRange_ShouldSucceed()
    {
        // Act
        var constraint = NumericRangeConstraint.Between(1, 100);

        // Assert
        constraint.Should().NotBeNull();
        constraint.Type.Should().Be("NUMERIC_RANGE");
    }

    [Fact]
    public void Create_WithMinGreaterThanMax_ShouldThrow()
    {
        // Act
        var act = () => NumericRangeConstraint.Between(100, 1);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_WithEqualMinAndMax_ShouldSucceed()
    {
        // Act
        var constraint = NumericRangeConstraint.Between(5, 5);

        // Assert
        constraint.IsValid(5).Should().BeTrue();
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(101, false)]
    [InlineData(-1, false)]
    public void IsValid_WithIntegerValue_ShouldValidateRange(int value, bool expected)
    {
        // Arrange
        var constraint = NumericRangeConstraint.Between(1, 100);

        // Act
        var result = constraint.IsValid(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsValid_WithNonIntegerValue_ShouldReturnFalse()
    {
        // Arrange
        var constraint = NumericRangeConstraint.Between(1, 100);

        // Act
        var stringResult = constraint.IsValid("50");
        var decimalResult = constraint.IsValid(50.5m);

        // Assert
        stringResult.Should().BeFalse();
        decimalResult.Should().BeFalse();
    }

    [Fact]
    public void Min_And_Max_ShouldBeExposed()
    {
        // Arrange
        var constraint = (NumericRangeConstraint)NumericRangeConstraint.Between(10, 20);

        // Act
        var min = constraint.Min;
        var max = constraint.Max;

        // Assert
        min.Should().Be(10);
        max.Should().Be(20);
    }
}
