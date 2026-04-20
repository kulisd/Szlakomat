using MediatR;
using Szlakomat.Products.Domain.Catalog.SelectionRules;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Application.Catalog.DefinePackageType;

/// <summary>
/// Command to define a new PackageType in the system.
/// </summary>
public record DefinePackageType(
    string ProductIdType, // "UUID", "ISBN", "GTIN"
    string ProductId, // Will be parsed to ProductIdentifier based on productIdType
    string Name,
    string Description,
    string TrackingStrategy, // "IDENTICAL", "INDIVIDUALLY_TRACKED", "BATCH_TRACKED", etc.
    IReadOnlySet<ISelectionRuleConfig> SelectionRules,
    IReadOnlyDictionary<string, string>? Metadata // Static properties: category, seasonal, etc.
) : IRequest<Result<string, IProductIdentifier>>;
