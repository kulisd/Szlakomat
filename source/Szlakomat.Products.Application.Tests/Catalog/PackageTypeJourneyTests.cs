using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application.Catalog.DefinePackageType;
using Szlakomat.Products.Application.Catalog.DefineProductType;
using Szlakomat.Products.Application.Catalog.FindByTrackingStrategy;
using Szlakomat.Products.Application.Tests.Assemblers;
using Szlakomat.Products.Application.Tests.Infrastructure;
using Szlakomat.Products.Domain.Catalog.SelectionRules;

namespace Szlakomat.Products.Application.Tests.Catalog;

/// <summary>
/// Journey: operator rejestruje pakiet turystyczny złożony z kilku atrakcji.
/// </summary>
public class PackageTypeJourneyTests
{
    private readonly IMediator _mediator;

    public PackageTypeJourneyTests()
    {
        _mediator = ServiceProviderFactory.Create().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task OperatorDefinesPackageWithSingleChoice_PackageIsPersisted()
    {
        // Arrange
        var attractionA = Guid.NewGuid().ToString();
        var attractionB = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(attractionA));
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(attractionB));

        var packageId = Guid.NewGuid().ToString();
        var command = new DefinePackageType(
            ProductIdType: "UUID",
            ProductId: packageId,
            Name: "Krakow Highlights Bundle",
            Description: "Choose one highlight attraction",
            TrackingStrategy: "INDIVIDUALLY_TRACKED",
            SelectionRules: new HashSet<ISelectionRuleConfig>
            {
                new SingleConfig(new HashSet<string> { attractionA, attractionB })
            },
            Metadata: new Dictionary<string, string> { ["city"] = "Krakow" });

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task OperatorDefinesPackageWithoutRules_CommandFails()
    {
        // Arrange
        var command = new DefinePackageType(
            ProductIdType: "UUID",
            ProductId: Guid.NewGuid().ToString(),
            Name: "Empty Package",
            Description: "No rules",
            TrackingStrategy: "INDIVIDUALLY_TRACKED",
            SelectionRules: new HashSet<ISelectionRuleConfig>(),
            Metadata: null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsFailure());
    }

    [Fact]
    public async Task OperatorDefinesPackageWithCompositeRule_CommandFailsGracefully()
    {
        // Arrange
        var inner = new SingleConfig(new HashSet<string> { Guid.NewGuid().ToString() });
        var command = new DefinePackageType(
            ProductIdType: "UUID",
            ProductId: Guid.NewGuid().ToString(),
            Name: "Composite Package",
            Description: "Uses composite AND rule",
            TrackingStrategy: "INDIVIDUALLY_TRACKED",
            SelectionRules: new HashSet<ISelectionRuleConfig>
            {
                new AndRuleConfig(new HashSet<ISelectionRuleConfig> { inner })
            },
            Metadata: null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsFailure());
    }
}
