using Szlakomat.Products.Architecture.Tests.SeedWork;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Instances;
using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Architecture.Tests.Visibility;

public class DomainVisibilityTests : TestBase
{
    [Fact]
    public void Builders_ShouldBe_Internal()
    {
        // Arrange
        var topLevelBuilders = DomainAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Builder")
                        && !t.IsNested
                        && !t.IsInterface
                        && !t.IsAbstract)
            .ToList();
        topLevelBuilders.Should().NotBeEmpty("there should be top-level builder classes");

        // Act
        var publicBuilders = topLevelBuilders.Where(t => t.IsPublic).ToList();

        // Assert
        publicBuilders.Should().BeEmpty(
            $"these builders should be internal: {string.Join(", ", publicBuilders.Select(t => t.Name))}");
    }

    [Fact]
    public void Repositories_ShouldBe_InternalInterfaces()
    {
        // Arrange
        var repositoryInterfaces = DomainAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .ToList();
        repositoryInterfaces.Should().NotBeEmpty("there should be repository interfaces in the domain");

        // Act
        var unexpectedPublic = repositoryInterfaces
            .Where(t => t.IsPublic)
            .ToList();

        // Assert
        unexpectedPublic.Should().BeEmpty(
            $"these repositories should be internal: {string.Join(", ", unexpectedPublic.Select(t => t.Name))}");
    }

    [Fact]
    public void FeatureConstraintImplementations_ShouldBe_Internal()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IFeatureValueConstraint))
            .And()
            .AreNotInterfaces()
            .Should()
            .NotBePublic()
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void AggregatesAndBuilders_ShouldHave_NoPublicConstructors()
    {
        // Arrange — top-level aggregate/builder classes (e.g. ProductType, PackageType, ProductBuilder, InstanceBuilder)
        var suspects = DomainAssembly.GetTypes()
            .Where(t => (t.Name.EndsWith("Type") || t.Name.EndsWith("Builder"))
                        && !t.IsAbstract
                        && !t.IsInterface
                        && !t.IsEnum
                        && !t.IsValueType
                        && !t.IsNested)
            .ToList();
        suspects.Should().NotBeEmpty("there should be aggregate/builder classes in the domain");

        // Act
        var offenders = suspects
            .Where(t => t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any())
            .Select(t => t.FullName)
            .ToList();

        // Assert
        offenders.Should().BeEmpty(
            $"aggregates and builders must be constructed via static factory methods (no public ctors): {string.Join(", ", offenders)}");
    }

    [Fact]
    public void DomainInterfaces_ShouldBe_Public()
    {
        // Arrange
        var coreInterfaces = new[]
        {
            typeof(IProduct),
            typeof(IInstance),
            typeof(IProductIdentifier),
            typeof(ISerialNumber),
            typeof(IApplicabilityConstraint),
            typeof(IFeatureValueConstraint)
        };

        // Act
        var nonPublic = coreInterfaces.Where(t => !t.IsPublic).ToList();

        // Assert
        nonPublic.Should().BeEmpty(
            $"these domain interfaces should be public: {string.Join(", ", nonPublic.Select(t => t.Name))}");
    }
}
