using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// Definicja obowiązkowej cechy.
/// </summary>
public record MandatoryFeature(string Name, IFeatureConstraintConfig Constraint);
