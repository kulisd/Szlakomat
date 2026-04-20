using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application.Catalog.FindProductType;
using Szlakomat.Products.Application.Tests.Assemblers;
using Szlakomat.Products.Application.Tests.Infrastructure;

namespace Szlakomat.Products.Application.Tests.Catalog;

/// <summary>
/// Journey: a city tourism operator registers sightseeing attractions in the product catalogue.
/// </summary>
public class CityAttractionCatalogJourneyTests
{
    private readonly IMediator _mediator;

    public CityAttractionCatalogJourneyTests()
    {
        _mediator = ServiceProviderFactory.Create().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task OperatorAddsSimpleAttraction_CatalogReturnsIdentifier()
    {
        // Arrange
        var command = CatalogCommandAssembler.SimpleAttraction(Guid.NewGuid().ToString());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task OperatorAddsGuidedAttraction_CatalogReturnsIdentifier()
    {
        // Arrange
        var command = CatalogCommandAssembler.GuidedAttraction(Guid.NewGuid().ToString());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task OperatorLooksUpRegisteredAttraction_SystemReturnsAttractionCard()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.SimpleAttraction(id));

        // Act
        var view = await _mediator.Send(new FindProductTypeCriteria(id));

        // Assert
        Assert.NotNull(view);
        Assert.Equal("Kosciuszko Mound", view.Name);
        Assert.Equal("IDENTICAL", view.TrackingStrategy);
    }
}
