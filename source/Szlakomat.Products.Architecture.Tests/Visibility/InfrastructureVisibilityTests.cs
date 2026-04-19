using Szlakomat.Products.Architecture.Tests.SeedWork;

namespace Szlakomat.Products.Architecture.Tests.Visibility;

public class InfrastructureVisibilityTests : TestBase
{
    [Fact]
    public void InMemoryRepositories_ShouldBe_Internal()
    {
        // Act
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .HaveNameStartingWith("InMemory")
            .Should()
            .NotBePublic()
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }
}
