using MediatR;
using Szlakomat.Products.Architecture.Tests.SeedWork;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Catalog.FeatureConstraints;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Architecture.Tests.NamingConventions;

public class NamingConventionTests : TestBase
{
    [Fact]
    public void Constraints_ShouldEndWith_Constraint()
    {
        // Arrange
        var applicabilityConstraints = DomainAssembly.GetTypes()
            .Where(t => typeof(IApplicabilityConstraint).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract
                        && t != typeof(ApplicabilityConstraint))
            .ToList();

        var featureConstraints = DomainAssembly.GetTypes()
            .Where(t => typeof(IFeatureValueConstraint).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract)
            .ToList();

        // Act
        var applicabilityNamingResults = applicabilityConstraints
            .Select(type => new
            {
                type,
                isValid = type.Name.EndsWith("Constraint")
            })
            .ToList();

        var featureNamingResults = featureConstraints
            .Select(type => new
            {
                type,
                isValid = type.Name.EndsWith("Constraint") || type.Name == "Unconstrained"
            })
            .ToList();

        // Assert
        foreach (var check in applicabilityNamingResults)
        {
            check.isValid.Should().BeTrue(
                $"{check.type.FullName} implements IApplicabilityConstraint");
        }

        foreach (var check in featureNamingResults)
        {
            check.isValid.Should().BeTrue(
                $"{check.type.FullName} implements IFeatureValueConstraint and should end with 'Constraint' or be 'Unconstrained'");
        }
    }

    [Fact]
    public void Identifiers_ShouldEndWith_ProductIdentifier()
    {
        // Arrange
        var identifiers = DomainAssembly.GetTypes()
            .Where(t => typeof(IProductIdentifier).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract)
            .ToList();

        // Act
        var namingResults = identifiers
            .Select(type => new
            {
                type,
                isValid = type.Name.EndsWith("ProductIdentifier")
            })
            .ToList();

        // Assert
        identifiers.Should().NotBeEmpty("there should be IProductIdentifier implementations");

        foreach (var check in namingResults)
        {
            check.isValid.Should().BeTrue(
                $"{check.type.FullName} implements IProductIdentifier");
        }
    }

    [Fact]
    public void SerialNumbers_ShouldEndWith_SerialNumber()
    {
        // Arrange
        var serialNumbers = DomainAssembly.GetTypes()
            .Where(t => typeof(ISerialNumber).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract)
            .ToList();

        // Act
        var namingResults = serialNumbers
            .Select(type => new
            {
                type,
                isValid = type.Name.EndsWith("SerialNumber")
            })
            .ToList();

        // Assert
        serialNumbers.Should().NotBeEmpty("there should be ISerialNumber implementations");

        foreach (var check in namingResults)
        {
            check.isValid.Should().BeTrue(
                $"{check.type.FullName} implements ISerialNumber");
        }
    }

    [Fact]
    public void Handlers_ShouldEndWith_Handler()
    {
        // Arrange
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        // Act
        var namingResults = handlers
            .Select(type => new
            {
                type,
                isValid = type.Name.EndsWith("Handler")
            })
            .ToList();

        // Assert
        handlers.Should().NotBeEmpty("there should be MediatR handlers");

        foreach (var check in namingResults)
        {
            check.isValid.Should().BeTrue(
                $"{check.type.FullName} implements IRequestHandler");
        }
    }

    [Fact]
    public void Controllers_ShouldEndWith_Controller()
    {
        // Act
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        AssertArchTestResult(result);
    }
}
