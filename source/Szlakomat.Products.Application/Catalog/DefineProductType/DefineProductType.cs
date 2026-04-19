using MediatR;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Application.Catalog.DefineProductType;

/// <summary>
/// Command to define a new ProductType in the system.
/// </summary>
public record DefineProductType(
    string ProductIdType, // "UUID", "ISBN", "GTIN"
    string ProductId, // Will be parsed to ProductIdentifier based on productIdType
    string Name,
    string Description,
    string Unit, // e.g., "pcs", "kg", "m²"
    string TrackingStrategy, // "IDENTICAL", "INDIVIDUALLY_TRACKED", "BATCH_TRACKED", etc.
    IReadOnlySet<MandatoryFeature>? MandatoryFeatures,
    IReadOnlySet<OptionalFeature>? OptionalFeatures,
    IReadOnlyDictionary<string, string>? Metadata // Static properties: category, seasonal, brand, etc.
) : IRequest<Result<string, IProductIdentifier>>;
