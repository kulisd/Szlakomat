using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Application;
using Szlakomat.Products.Infrastructure;

namespace Szlakomat.Products.Architecture.Tests.SeedWork;

public abstract class TestBase
{
    protected static Assembly DomainAssembly => typeof(IProduct).Assembly;
    protected static Assembly ApplicationAssembly => typeof(ProductModule).Assembly;
    protected static Assembly InfrastructureAssembly => typeof(ProductServiceExtensions).Assembly;
    protected static Assembly ApiAssembly => typeof(Szlakomat.Products.Api.Controllers.ProductsController).Assembly;

    protected static void AssertArchTestResult(TestResult result)
    {
        var failingTypes = result.FailingTypes ?? [];
        failingTypes.Should().BeNullOrEmpty(
            $"the following types violate the architecture rule: {string.Join(", ", failingTypes.Select(t => t.FullName))}");
    }

    protected static void AssertAreImmutable(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var mutableProperties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetSetMethod() != null && !IsInitOnly(p))
                .ToList();

            mutableProperties.Should().BeEmpty(
                $"{type.Name} should be immutable but has mutable properties: " +
                $"{string.Join(", ", mutableProperties.Select(p => p.Name))}");
        }
    }

    private static bool IsInitOnly(PropertyInfo property)
    {
        var setMethod = property.GetSetMethod();
        if (setMethod == null) return false;
        // Init-only setters have IsExternalInit as a required custom modifier on the return parameter
        return setMethod.ReturnParameter
            .GetRequiredCustomModifiers()
            .Any(t => t.Name == "IsExternalInit");
    }
}
