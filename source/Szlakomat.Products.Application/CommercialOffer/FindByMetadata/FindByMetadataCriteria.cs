using MediatR;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindByMetadata;

/// <summary>
/// Criteria to find CatalogEntries by metadata key (and optionally value).
/// </summary>
public record FindByMetadataCriteria(string Key, string? Value) : IRequest<IReadOnlySet<CatalogEntryView>>;
