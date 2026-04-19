using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// Definicja opcjonalnej cechy.
/// </summary>
public record OptionalFeature(string Name, IFeatureConstraintConfig Constraint);
