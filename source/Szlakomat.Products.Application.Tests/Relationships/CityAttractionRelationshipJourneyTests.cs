using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application.Relationships.DefineRelationship;
using Szlakomat.Products.Application.Relationships.FindRelationsFrom;
using Szlakomat.Products.Application.Relationships.RemoveRelationship;
using Szlakomat.Products.Application.Tests.Assemblers;
using Szlakomat.Products.Application.Tests.Infrastructure;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Tests.Relationships;

/// <summary>
/// Journey: a city operator defines relationships between sightseeing attractions.
/// </summary>
public class CityAttractionRelationshipJourneyTests
{
    private readonly IMediator _mediator;

    public CityAttractionRelationshipJourneyTests()
    {
        _mediator = ServiceProviderFactory.Create().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task OperatorMarksWawelCastleAsComplementedByWawelCathedral_SystemReturnsRelationshipId()
    {
        // Arrange
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(cathedralId));

        // Act
        var result = await _mediator.Send(new DefineRelationship("UUID", castleId, "UUID", cathedralId, "COMPLEMENTED_BY"));

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task TouristAsksWhatElseToVisitNearWawel_SystemShowsComplementaryAttractions()
    {
        // Arrange
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(cathedralId));
        await _mediator.Send(new DefineRelationship("UUID", castleId, "UUID", cathedralId, "COMPLEMENTED_BY"));

        // Act
        var relations = await _mediator.Send(
            new FindRelationsFromQuery(UuidProductIdentifier.Of(castleId), ProductRelationshipType.ComplementedBy));

        // Assert
        Assert.NotEmpty(relations);
        Assert.Contains(relations, r => r.ToProductId == cathedralId);
    }

    [Fact]
    public async Task OperatorDefinesRelationshipBetweenInspireIdentifiedHeritageSites_SystemReturnsRelationshipId()
    {
        // Arrange — two products registered with INSPIRE identifiers (used in the Wawel seed)
        var wawelCastleInspire = "PL.1.9.ZIPOZ.NID_N_12_BK.217616";
        var wawelCathedralInspire = "PL.1.9.ZIPOZ.NID_N_12_KS.217617";
        await _mediator.Send(new Szlakomat.Products.Application.Catalog.DefineProductType.DefineProductType(
            "INSPIRE", wawelCastleInspire,
            "Zamek Królewski na Wawelu",
            "Zamek — rejestr NID",
            "pcs", "IDENTICAL",
            null, null,
            new Dictionary<string, string> { ["city"] = "Krakow" }));
        await _mediator.Send(new Szlakomat.Products.Application.Catalog.DefineProductType.DefineProductType(
            "INSPIRE", wawelCathedralInspire,
            "Katedra na Wawelu",
            "Katedra — rejestr NID",
            "pcs", "IDENTICAL",
            null, null,
            new Dictionary<string, string> { ["city"] = "Krakow" }));

        // Act
        var result = await _mediator.Send(new DefineRelationship(
            "INSPIRE", wawelCastleInspire,
            "INSPIRE", wawelCathedralInspire,
            "COMPLEMENTED_BY"));

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task OperatorRemovesRecommendationDuringRenovation_TouristSeesNoRelationships()
    {
        // Arrange
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(cathedralId));
        var defineResult = await _mediator.Send(new DefineRelationship("UUID", castleId, "UUID", cathedralId, "COMPLEMENTED_BY"));
        var relationshipId = defineResult.GetSuccess()!.AsString();

        // Act
        await _mediator.Send(new RemoveRelationship(relationshipId));
        var relations = await _mediator.Send(
            new FindRelationsFromQuery(UuidProductIdentifier.Of(castleId), null));

        // Assert
        Assert.Empty(relations);
    }
}
