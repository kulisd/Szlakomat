namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

public record DateRangeConfig(string From, string To) : IFeatureConstraintConfig;
