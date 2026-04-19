namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

public record NumericRangeConfig(int Min, int Max) : IFeatureConstraintConfig;
