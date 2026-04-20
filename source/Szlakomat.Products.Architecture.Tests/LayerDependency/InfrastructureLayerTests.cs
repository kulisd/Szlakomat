using Szlakomat.Products.Architecture.Tests.SeedWork;

namespace Szlakomat.Products.Architecture.Tests.LayerDependency;

public class InfrastructureLayerTests : TestBase
{
    [Fact]
    public void Infrastructure_ShouldNotReference_ApiLayer()
    {
        // Act
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Api")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }
}
