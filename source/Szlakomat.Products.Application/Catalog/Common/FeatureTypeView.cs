namespace Szlakomat.Products.Application.Catalog.Common;

/// <summary>
/// View of a ProductFeatureType - definition of a configurable characteristic.
/// </summary>
public record FeatureTypeView(
    string Name,
    string ValueType, // "TEXT", "INTEGER", "DECIMAL", "DATE", "BOOLEAN"
    string ConstraintType, // "ALLOWED_VALUES", "NUMERIC_RANGE", "REGEX", etc.
    IReadOnlyDictionary<string, object>? ConstraintConfig,
    string ConstraintDescription
);
