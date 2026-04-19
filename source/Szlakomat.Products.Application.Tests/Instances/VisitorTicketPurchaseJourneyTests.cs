using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application.Instances.CreateProductInstance;
using Szlakomat.Products.Application.Instances.FindInstance;
using Szlakomat.Products.Application.Tests.Assemblers;
using Szlakomat.Products.Application.Tests.Infrastructure;

namespace Szlakomat.Products.Application.Tests.Instances;

/// <summary>
/// Journey: a tourist books an entry to a city attraction.
/// </summary>
public class TouristAttractionVisitJourneyTests
{
    private readonly IMediator _mediator;

    public TouristAttractionVisitJourneyTests()
    {
        _mediator = ServiceProviderFactory.Create().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task TouristBooksTimedEntry_SystemReturnsReservationId()
    {
        // Arrange
        var attractionId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.TimedEntryAttraction(attractionId));
        var command = new CreateProductInstance(
            attractionId, "RYNEK-UNDERGROUND-2025-04-15-1030", null, "1", "pcs", null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task OrganizerBooksGroupVisit_SystemReturnsGroupReservationId()
    {
        // Arrange
        var attractionId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.GroupAttraction(attractionId));
        var groupId = Guid.NewGuid().ToString();
        var command = new CreateProductInstance(attractionId, null, groupId, "30", "pcs", null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.NotNull(result.GetSuccess());
    }

    [Fact]
    public async Task StaffValidatesGuestReservationAtEntrance_SystemReturnsBookingDetails()
    {
        // Arrange
        var attractionId = Guid.NewGuid().ToString();
        await _mediator.Send(CatalogCommandAssembler.TimedEntryAttraction(attractionId));
        var bookingCode = "RYNEK-UNDERGROUND-2025-05-01-1200";
        var createResult = await _mediator.Send(new CreateProductInstance(
            attractionId, bookingCode, null, "1", "pcs", null));
        var instanceId = createResult.GetSuccess()!.ToString();

        // Act
        var view = await _mediator.Send(new FindInstanceQuery(instanceId));

        // Assert
        Assert.NotNull(view);
        Assert.Equal(attractionId, view.ProductId);
        Assert.Equal(bookingCode, view.SerialNumber);
    }
}
