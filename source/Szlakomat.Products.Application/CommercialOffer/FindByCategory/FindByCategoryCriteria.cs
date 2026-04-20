using MediatR;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindByCategory;

/// <summary>
/// Criteria to find all CatalogEntries in a given category.
/// </summary>
public record FindByCategoryCriteria(string Category) : IRequest<IReadOnlySet<CatalogEntryView>>;
