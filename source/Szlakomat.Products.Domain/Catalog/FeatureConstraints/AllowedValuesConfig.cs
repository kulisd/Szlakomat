namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

public record AllowedValuesConfig(IReadOnlySet<string> AllowedValues) : IFeatureConstraintConfig;
