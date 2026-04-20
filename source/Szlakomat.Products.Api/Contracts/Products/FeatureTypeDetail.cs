namespace Szlakomat.Products.Api.Contracts.Products;

public record FeatureTypeDetail(
    string Name,
    string ValueType,
    string ConstraintType,
    string ConstraintDescription
);