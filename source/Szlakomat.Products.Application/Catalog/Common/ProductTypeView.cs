namespace Szlakomat.Products.Application.Catalog.Common;

/// <summary>
/// View of a ProductType - business definition of a product.
/// </summary>
public record ProductTypeView(
    string ProductId,
    string Name,
    string Description,
    string Unit,
    string TrackingStrategy,
    IReadOnlySet<FeatureTypeView>? MandatoryFeatures,
    IReadOnlySet<FeatureTypeView>? OptionalFeatures
);
