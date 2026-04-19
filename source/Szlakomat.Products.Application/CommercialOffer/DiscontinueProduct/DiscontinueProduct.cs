using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Application.CommercialOffer.DiscontinueProduct;

/// <summary>
/// Command to discontinue a product from the offer.
/// </summary>
public record DiscontinueProduct(
    string CatalogEntryId, // Will be parsed to CatalogEntryId
    DateOnly DiscontinuationDate
) : IRequest<Result<string, CatalogEntryId>>;
