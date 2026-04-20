using Szlakomat.Products.Architecture.Tests.SeedWork;

namespace Szlakomat.Products.Architecture.Tests.LayerDependency;

public class ApplicationLayerTests : TestBase
{
    [Fact]
    public void Application_ShouldNotReference_InfrastructureLayer()
    {
        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Infrastructure")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Application_ShouldNotReference_ApiLayer()
    {
        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Api")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Application_ShouldNotReference_AspNetCore()
    {
        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.AspNetCore")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }
}
