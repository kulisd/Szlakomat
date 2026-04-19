using Szlakomat.Products.Architecture.Tests.SeedWork;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Architecture.Tests.Immutability;

public class ValueObjectImmutabilityTests : TestBase
{
    [Fact]
    public void ValueObjects_ShouldBe_Immutable()
    {
        // Arrange
        var valueObjectTypes = new[]
        {
            typeof(ProductName),
            typeof(ProductDescription),
            typeof(UuidProductIdentifier),
            typeof(GtinProductIdentifier),
            typeof(IsbnProductIdentifier),
            typeof(ImeiSerialNumber),
            typeof(VinSerialNumber),
            typeof(TextualSerialNumber)
        };

        // Act & Assert
        AssertAreImmutable(valueObjectTypes);
    }

    [Fact]
    public void ApplicabilityConstraints_ShouldBe_Immutable()
    {
        // Arrange
        var constraintTypes = DomainAssembly.GetTypes()
            .Where(t => typeof(IApplicabilityConstraint).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract)
            .ToList();

        // Act
        constraintTypes.Should().NotBeEmpty("there should be applicability constraint implementations");

        // Assert
        AssertAreImmutable(constraintTypes);
    }

    [Fact]
    public void Result_ShouldBe_Immutable()
    {
        // Arrange
        var resultType = typeof(Result<,>);
        var closedResult = typeof(Result<string, string>);
        var nestedTypes = closedResult.GetNestedTypes(BindingFlags.Public);

        // Act
        var isAbstract = resultType.IsAbstract;
        var mutablePropsPerType = nestedTypes
            .Where(t => !t.IsAbstract)
            .Select(nested => new
            {
                typeName = nested.Name,
                mutableProps = nested
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanWrite && p.GetSetMethod() != null)
                    .ToList()
            })
            .ToList();

        // Assert
        isAbstract.Should().BeTrue("Result should be abstract (sealed hierarchy)");

        foreach (var check in mutablePropsPerType)
        {
            check.mutableProps.Should().BeEmpty(
                $"{check.typeName} should be immutable but has mutable properties: " +
                $"{string.Join(", ", check.mutableProps.Select(p => p.Name))}");
        }
    }
}
