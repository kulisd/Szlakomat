using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application.CommercialOffer.AddToOffer;
using Szlakomat.Products.Application.CommercialOffer.SearchCatalog;
using Szlakomat.Products.Application.Tests.Assemblers;
using Szlakomat.Products.Application.Tests.Infrastructure;

namespace Szlakomat.Products.Application.Tests.CommercialOffer;

/// <summary>
/// Journey: a city puts sightseeing attractions into its tourism commercial offer.
/// </summary>
public class CityAttractionOfferingJourneyTests
{
    private readonly IMediator _mediator;

    public CityAttractionOfferingJourneyTests()
    {
        _mediator = ServiceProviderFactory.Create().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task CityPublishesAttractionInSeasonalOffer_SystemReturnsCatalogEntryId()
    {
        // Arrange
        var castleId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        var command = new AddToOffer(
            castleId,
            "Wawel Castle — guided tour",
            "Royal residence on Wawel Hill — group tours with a licensed guide",
            new HashSet<string> { "monuments", "krakow" },
            new DateOnly(2025, 4, 1),
            new DateOnly(2025, 10, 31),
            null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task TouristBrowsesAllAttractions_PublishedAttractionAppearsInList()
    {
        // Arrange
        var castleId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        await _mediator.Send(new AddToOffer(
            castleId, "Wawel Castle", "Guided tour", new HashSet<string> { "monuments" },
            null, null, null));

        // Act
        var entries = await _mediator.Send(SearchCatalogCriteria.All());

        // Assert
        Assert.Contains(entries, e => e.ProductTypeId == castleId);
    }

    [Fact]
    public async Task TouristFiltersByMonumentsCategory_SeesOnlyTaggedAttractions()
    {
        // Arrange
        var castleId = Guid.NewGuid().ToString();
        var uncategorisedId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GuidedAttraction(castleId));
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(uncategorisedId));
        await _mediator.Send(new AddToOffer(
            castleId, "Wawel Castle", "Guided tour", new HashSet<string> { "monuments" },
            null, null, null));
        await _mediator.Send(new AddToOffer(
            uncategorisedId, "New place", "No category yet", null, null, null, null));

        // Act
        var entries = await _mediator.Send(
            SearchCatalogCriteria.ByCategories(new HashSet<string> { "monuments" }));

        // Assert
        Assert.Contains(entries, e => e.ProductTypeId == castleId);
        Assert.DoesNotContain(entries, e => e.ProductTypeId == uncategorisedId);
    }
}
