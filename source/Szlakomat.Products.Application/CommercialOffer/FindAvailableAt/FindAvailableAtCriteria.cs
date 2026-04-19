using MediatR;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindAvailableAt;

/// <summary>
/// Criteria to find all CatalogEntries available at a specific date.
/// </summary>
public record FindAvailableAtCriteria(DateOnly Date) : IRequest<IReadOnlySet<CatalogEntryView>>;
