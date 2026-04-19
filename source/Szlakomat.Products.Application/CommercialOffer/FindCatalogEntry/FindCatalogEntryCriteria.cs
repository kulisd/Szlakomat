using MediatR;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindCatalogEntry;

/// <summary>
/// Criteria to find a CatalogEntry by its identifier.
/// </summary>
public record FindCatalogEntryCriteria(string CatalogEntryId) : IRequest<CatalogEntryView?>;
