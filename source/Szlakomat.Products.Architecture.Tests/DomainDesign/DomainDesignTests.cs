using Szlakomat.Products.Architecture.Tests.SeedWork;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Architecture.Tests.DomainDesign;

public class DomainDesignTests : TestBase
{
    [Fact]
    public void ValueObjects_ShouldBe_Records()
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

        // Act
        var recordChecks = valueObjectTypes.Select(type => new
        {
            type,
            isRecord = type.GetMethods().Any(m => m.Name == "<Clone>$")
        }).ToList();

        // Assert
        foreach (var check in recordChecks)
        {
            check.isRecord.Should().BeTrue($"{check.type.Name} should be a record");
        }
    }

    [Fact]
    public void DomainClasses_ShouldNotUse_SystemDataAnnotations()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("System.ComponentModel.DataAnnotations")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Domain_ShouldNotReference_AspNetCore()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.AspNetCore")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Domain_ShouldNotReference_EntityFramework()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }

    [Fact]
    public void Interfaces_ShouldNotHave_ImplementationDetails()
    {
        // Arrange
        var domainInterfaces = DomainAssembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .ToList();

        // Act & Assert
        domainInterfaces.Should().NotBeEmpty("there should be public domain interfaces");

        foreach (var iface in domainInterfaces)
        {
            var methods = iface.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                method.ReturnType.Namespace.Should().NotStartWith("Microsoft.EntityFrameworkCore",
                    $"{iface.Name}.{method.Name} return type should not reference EF");
                method.ReturnType.Namespace.Should().NotStartWith("Microsoft.AspNetCore",
                    $"{iface.Name}.{method.Name} return type should not reference ASP.NET");
            }
        }
    }
}
