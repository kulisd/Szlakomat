namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

public record RegexConfig(string Pattern) : IFeatureConstraintConfig;
