using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Domain.Tests.Domain.ValueObjects;

public class ResultTests
{
    [Fact]
    public void SuccessOf_ShouldCreateSuccessResult()
    {
        // Act
        var result = Result<string, int>.SuccessOf(42);

        // Assert
        result.IsSuccess().Should().BeTrue();
        result.IsFailure().Should().BeFalse();
        result.GetSuccess().Should().Be(42);
    }

    [Fact]
    public void FailureOf_ShouldCreateFailureResult()
    {
        // Act
        var result = Result<string, int>.FailureOf("error");

        // Assert
        result.IsFailure().Should().BeTrue();
        result.IsSuccess().Should().BeFalse();
        result.GetFailure().Should().Be("error");
    }

    [Fact]
    public void Map_OnSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<string, int>.SuccessOf(10);

        // Act
        var mapped = result.Map(x => x * 2);

        // Assert
        mapped.IsSuccess().Should().BeTrue();
        mapped.GetSuccess().Should().Be(20);
    }

    [Fact]
    public void Map_OnFailure_ShouldPreserveFailure()
    {
        // Arrange
        var result = Result<string, int>.FailureOf("error");

        // Act
        var mapped = result.Map(x => x * 2);

        // Assert
        mapped.IsFailure().Should().BeTrue();
        mapped.GetFailure().Should().Be("error");
    }

    [Fact]
    public void FlatMap_OnSuccess_ShouldChainResults()
    {
        // Arrange
        var result = Result<string, int>.SuccessOf(10);

        // Act
        var chained = result.FlatMap(x =>
            x > 5
                ? Result<string, string>.SuccessOf($"big: {x}")
                : Result<string, string>.FailureOf("too small"));

        // Assert
        chained.IsSuccess().Should().BeTrue();
        chained.GetSuccess().Should().Be("big: 10");
    }

    [Fact]
    public void FlatMap_OnFailure_ShouldPreserveFailure()
    {
        // Arrange
        var result = Result<string, int>.FailureOf("original error");

        // Act
        var chained = result.FlatMap(x => Result<string, string>.SuccessOf($"val: {x}"));

        // Assert
        chained.IsFailure().Should().BeTrue();
        chained.GetFailure().Should().Be("original error");
    }

    [Fact]
    public void Fold_OnSuccess_ShouldApplyRightMapper()
    {
        // Arrange
        var result = Result<string, int>.SuccessOf(42);

        // Act
        var folded = result.Fold(
            failure => $"FAIL: {failure}",
            success => $"OK: {success}");

        // Assert
        folded.Should().Be("OK: 42");
    }

    [Fact]
    public void Fold_OnFailure_ShouldApplyLeftMapper()
    {
        // Arrange
        var result = Result<string, int>.FailureOf("oops");

        // Act
        var folded = result.Fold(
            failure => $"FAIL: {failure}",
            success => $"OK: {success}");

        // Assert
        folded.Should().Be("FAIL: oops");
    }

    [Fact]
    public void Combine_BothSuccess_ShouldCombineValues()
    {
        // Arrange
        var first = Result<string, int>.SuccessOf(10);
        var second = Result<string, int>.SuccessOf(20);

        // Act
        var combined = first.Combine(second,
            (f1, f2) => $"{f1},{f2}",
            (s1, s2) => s1 + s2);

        // Assert
        combined.IsSuccess().Should().BeTrue();
        combined.GetSuccess().Should().Be(30);
    }

    [Fact]
    public void Combine_FirstFailure_ShouldReturnFailure()
    {
        // Arrange
        var first = Result<string, int>.FailureOf("err1");
        var second = Result<string, int>.SuccessOf(20);

        // Act
        var combined = first.Combine(second,
            (f1, f2) => $"{f1},{f2}",
            (s1, s2) => s1 + s2);

        // Assert
        combined.IsFailure().Should().BeTrue();
    }

    [Fact]
    public void Composite_ShouldAccumulateSuccesses()
    {
        // Act
        var composite = Result<string, int>.Composite()
            .Accumulate(Result<string, int>.SuccessOf(1))
            .Accumulate(Result<string, int>.SuccessOf(2))
            .Accumulate(Result<string, int>.SuccessOf(3));

        // Assert
        composite.IsSuccess().Should().BeTrue();

        var listResult = composite.ToResult();
        listResult.GetSuccess().Should().BeEquivalentTo(new List<int> { 1, 2, 3 });
    }

    [Fact]
    public void Composite_WithFailure_ShouldStopAccumulating()
    {
        // Act
        var composite = Result<string, int>.Composite()
            .Accumulate(Result<string, int>.SuccessOf(1))
            .Accumulate(Result<string, int>.FailureOf("error"))
            .Accumulate(Result<string, int>.SuccessOf(3));

        // Assert
        composite.IsFailure().Should().BeTrue();
    }

    [Fact]
    public void Peek_OnSuccess_ShouldCallSuccessConsumer()
    {
        // Arrange
        int captured = 0;
        var result = Result<string, int>.SuccessOf(42);

        // Act
        result.Peek(s => captured = s, _ => { });

        // Assert
        captured.Should().Be(42);
    }

    [Fact]
    public void MapFailure_ShouldTransformFailureValue()
    {
        // Arrange
        var result = Result<string, int>.FailureOf("err");

        // Act
        var mapped = result.MapFailure(f => f.Length);

        // Assert
        mapped.IsFailure().Should().BeTrue();
        mapped.GetFailure().Should().Be(3);
    }
}
