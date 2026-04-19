namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

public record DecimalRangeConfig(string Min, string Max) : IFeatureConstraintConfig;
