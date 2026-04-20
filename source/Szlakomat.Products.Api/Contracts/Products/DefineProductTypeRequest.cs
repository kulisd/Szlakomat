namespace Szlakomat.Products.Api.Contracts.Products;

public record DefineProductTypeRequest(
    string ProductIdType,
    string ProductId,
    string Name,
    string Description,
    string Unit,
    string TrackingStrategy,
    List<FeatureDefinition>? MandatoryFeatures,
    List<FeatureDefinition>? OptionalFeatures,
    Dictionary<string, string>? Metadata
);