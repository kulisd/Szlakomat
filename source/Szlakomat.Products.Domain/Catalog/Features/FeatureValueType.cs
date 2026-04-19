namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// Defines the safe set of data types that can be used for product feature values.
/// Each type knows how to convert between its runtime representation and String (for persistence).
///
/// This enum restricts feature values to a well-defined set of types, preventing
/// arbitrary classes from being used as feature values.
/// </summary>
public enum FeatureValueType
{
    Text,
    Integer,
    Decimal,
    Date,
    Boolean
}
