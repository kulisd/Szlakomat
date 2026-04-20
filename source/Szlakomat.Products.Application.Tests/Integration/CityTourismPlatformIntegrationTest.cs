using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application.CommercialOffer.AddToOffer;
using Szlakomat.Products.Application.CommercialOffer.SearchCatalog;
using Szlakomat.Products.Application.Instances.CreateProductInstance;
using Szlakomat.Products.Application.Instances.FindInstance;
using Szlakomat.Products.Application.Relationships.DefineRelationship;
using Szlakomat.Products.Application.Relationships.FindRelationsFrom;
using Szlakomat.Products.Application.Relationships.RemoveRelationship;
using Szlakomat.Products.Application.Tests.Assemblers;
using Szlakomat.Products.Application.Tests.Infrastructure;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Instances;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Tests.Integration;

/// <summary>
/// Cross-context integration tests for the city tourism platform.
/// Each test spins up its own ServiceProvider (isolated in-memory repos)
/// and exercises at least two bounded contexts in a single scenario.
/// Contexts covered: Catalog · CommercialOffer · Relationships · Instances.
/// </summary>
public class CityTourismPlatformIntegrationTests
{
    // ── helpers ─────────────────────────────────────────────────────────────────

    private static IMediator BuildMediator() =>
        ServiceProviderFactory.Create().GetRequiredService<IMediator>();

    private static async Task RegisterBothAttractions(IMediator mediator, string castleId, string cathedralId)
    {
        await mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        await mediator.Send(CatalogCommandAssembler.SimpleAttraction(cathedralId));
    }

    // ── 1. Catalog ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Catalog_OperatorRegistersGuidedAndSimpleAttraction_BothSavedSuccessfully()
    {
        // Arrange
        var mediator = BuildMediator();
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();

        // Act
        var castleResult = await mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        var cathedralResult = await mediator.Send(CatalogCommandAssembler.SimpleAttraction(cathedralId));

        // Assert
        Assert.True(castleResult.IsSuccess());
        Assert.True(cathedralResult.IsSuccess());
    }

    // ── 2. Catalog → CommercialOffer ─────────────────────────────────────────────

    [Fact]
    public async Task CommercialOffer_BothAttractionsPublished_AppearInFullCatalogListing()
    {
        // Arrange
        var mediator = BuildMediator();
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await RegisterBothAttractions(mediator, castleId, cathedralId);
        await mediator.Send(new AddToOffer(
            castleId, "Wawel Castle", "Guided tour with licensed guide",
            new HashSet<string> { "monuments" }, new DateOnly(2025, 4, 1), new DateOnly(2025, 10, 31), null));
        await mediator.Send(new AddToOffer(
            cathedralId, "Wawel Cathedral", "Free entry, self-guided visit",
            new HashSet<string> { "monuments" }, new DateOnly(2025, 4, 1), new DateOnly(2025, 10, 31), null));

        // Act
        var entries = await mediator.Send(SearchCatalogCriteria.All());

        // Assert
        Assert.Contains(entries, e => e.ProductTypeId == castleId);
        Assert.Contains(entries, e => e.ProductTypeId == cathedralId);
    }

    [Fact]
    public async Task CommercialOffer_FilterByMonumentsCategory_ReturnsBothWawelAttractions()
    {
        // Arrange
        var mediator = BuildMediator();
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await RegisterBothAttractions(mediator, castleId, cathedralId);
        await mediator.Send(new AddToOffer(
            castleId, "Wawel Castle", "Guided tour with licensed guide",
            new HashSet<string> { "monuments" }, null, null, null));
        await mediator.Send(new AddToOffer(
            cathedralId, "Wawel Cathedral", "Free entry, self-guided visit",
            new HashSet<string> { "monuments" }, null, null, null));

        // Act
        var entries = await mediator.Send(
            SearchCatalogCriteria.ByCategories(new HashSet<string> { "monuments" }));

        // Assert
        Assert.Contains(entries, e => e.ProductTypeId == castleId);
        Assert.Contains(entries, e => e.ProductTypeId == cathedralId);
    }

    // ── 3. Catalog → Relationships ───────────────────────────────────────────────

    [Fact]
    public async Task Relationships_OperatorMarksAttractionAsComplementary_TouristSeesRecommendation()
    {
        // Arrange
        var mediator = BuildMediator();
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await RegisterBothAttractions(mediator, castleId, cathedralId);
        await mediator.Send(new DefineRelationship("UUID", castleId, "UUID", cathedralId, "COMPLEMENTED_BY"));

        // Act
        var relations = await mediator.Send(
            new FindRelationsFromQuery(UuidProductIdentifier.Of(castleId), ProductRelationshipType.ComplementedBy));

        // Assert
        Assert.NotEmpty(relations);
        Assert.Contains(relations, r => r.ToProductId == cathedralId);
    }

    // ── 4. Catalog → Instances ───────────────────────────────────────────────────

    [Fact]
    public async Task Instances_OrganizerBooksGroupVisit_InstanceStoredWithCorrectProductAndBatch()
    {
        // Arrange
        var mediator = BuildMediator();
        var castleId = Guid.NewGuid().ToString();
        var groupBookingId = Guid.NewGuid().ToString();
        await mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        var createResult = await mediator.Send(new CreateProductInstance(
            castleId, null, groupBookingId, "25", "pcs",
            new HashSet<FeatureInstanceConfig> { new("guide_language", "PL") }));
        var instanceId = createResult.GetSuccess()!.ToString();

        // Act
        var view = await mediator.Send(new FindInstanceQuery(instanceId));

        // Assert
        Assert.NotNull(view);
        Assert.Equal(castleId, view.ProductId);
        Assert.Equal(groupBookingId, view.BatchId);
    }

    // ── 5. Catalog → Relationships (cleanup) ─────────────────────────────────────

    [Fact]
    public async Task Relationships_CathedralClosedForRenovation_RemovedRelationshipNoLongerVisible()
    {
        // Arrange
        var mediator = BuildMediator();
        var castleId = Guid.NewGuid().ToString();
        var cathedralId = Guid.NewGuid().ToString();
        await RegisterBothAttractions(mediator, castleId, cathedralId);
        var defineResult = await mediator.Send(new DefineRelationship("UUID", castleId, "UUID", cathedralId, "COMPLEMENTED_BY"));
        var relationshipId = defineResult.GetSuccess()!.AsString();

        // Act
        await mediator.Send(new RemoveRelationship(relationshipId));
        var relations = await mediator.Send(
            new FindRelationsFromQuery(UuidProductIdentifier.Of(castleId), null));

        // Assert
        Assert.Empty(relations);
    }
}
