namespace Szlakomat.Products.Api.Contracts.Products;

public record FeatureDefinition(
    string Name,
    string ConstraintType,
    Dictionary<string, object>? ConstraintParams
);