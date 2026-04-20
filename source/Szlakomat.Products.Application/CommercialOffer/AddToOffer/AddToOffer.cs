using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Application.CommercialOffer.AddToOffer;

/// <summary>
/// Command to add a ProductType to the commercial offer.
/// </summary>
public record AddToOffer(
    string ProductTypeId, // Will be parsed to ProductIdentifier
    string DisplayName,
    string Description,
    IReadOnlySet<string>? Categories,
    DateOnly? AvailableFrom,
    DateOnly? AvailableUntil,
    IReadOnlyDictionary<string, string>? Metadata
) : IRequest<Result<string, CatalogEntryId>>;
