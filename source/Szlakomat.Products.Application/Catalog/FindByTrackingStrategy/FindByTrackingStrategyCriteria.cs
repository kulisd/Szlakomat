using MediatR;
using Szlakomat.Products.Application.Catalog.Common;

namespace Szlakomat.Products.Application.Catalog.FindByTrackingStrategy;

/// <summary>
/// Criteria to find all ProductTypes matching given tracking strategy.
/// </summary>
public record FindByTrackingStrategyCriteria(
    string TrackingStrategy // "IDENTICAL", "INDIVIDUALLY_TRACKED", "BATCH_TRACKED", etc.
) : IRequest<IReadOnlySet<ProductTypeView>>;
