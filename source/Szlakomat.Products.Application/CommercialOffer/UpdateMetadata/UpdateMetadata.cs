using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Application.CommercialOffer.UpdateMetadata;

/// <summary>
/// Command to update catalog entry metadata.
/// </summary>
public record UpdateMetadata(
    string CatalogEntryId, // Will be parsed to CatalogEntryId
    IReadOnlyDictionary<string, string> Metadata
) : IRequest<Result<string, CatalogEntryId>>;
