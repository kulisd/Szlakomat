using Szlakomat.Products.Architecture.Tests.SeedWork;

namespace Szlakomat.Products.Architecture.Tests.LayerDependency;

public class DomainLayerTests : TestBase
{
    [Fact]
    public void Domain_ShouldNotReference_ApplicationLayer()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Application")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Domain_ShouldNotReference_InfrastructureLayer()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Infrastructure")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Domain_ShouldNotReference_ApiLayer()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Api")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Domain_ShouldNotReference_MediatR()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }
}
