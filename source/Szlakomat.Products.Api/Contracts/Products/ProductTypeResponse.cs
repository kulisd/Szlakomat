namespace Szlakomat.Products.Api.Contracts.Products;

public record ProductTypeResponse(
    string ProductId,
    string Name,
    string Description,
    string Unit,
    string TrackingStrategy,
    List<FeatureTypeDetail>? MandatoryFeatures,
    List<FeatureTypeDetail>? OptionalFeatures
);