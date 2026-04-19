using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Defines constraints on product feature values, including type validation and value validation.
///
/// Each constraint:
/// - Specifies the expected value type (TEXT, INTEGER, DECIMAL, DATE, BOOLEAN)
/// - Has a string identifier for persistence/deserialization
/// - Validates whether a given value satisfies the constraint
/// - Provides conversion between objects and String representation for persistence
/// - Describes the constraint in human-readable form
///
/// Implementations include:
/// - AllowedValuesConstraint: restricts to a set of allowed values
/// - NumericRangeConstraint: restricts integers to a range
/// - DecimalRangeConstraint: restricts decimals to a range
/// - RegexConstraint: validates text against a regex pattern
/// - DateRangeConstraint: restricts dates to a range
/// - UnconstrainedConstraint: accepts any value of the specified type
/// </summary>
public interface IFeatureValueConstraint
{
    /// <summary>
    /// Returns the type of values this constraint applies to.
    /// </summary>
    FeatureValueType ValueType { get; }

    /// <summary>
    /// Returns the constraint type identifier for persistence/deserialization.
    /// Examples: "ALLOWED_VALUES", "NUMERIC_RANGE", "REGEX", "DATE_RANGE", "UNCONSTRAINED"
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Validates whether the given value satisfies this constraint.
    /// The value must be of the correct type (checked via ValueType) and meet
    /// the constraint's specific requirements.
    /// </summary>
    /// <param name="value">the value to validate</param>
    /// <returns>true if the value is valid, false otherwise</returns>
    bool IsValid(object value);

    /// <summary>
    /// Returns a human-readable description of this constraint.
    /// Example: "one of: {red, blue, green}" or "integer between 1 and 100"
    /// </summary>
    string Desc { get; }

    /// <summary>
    /// Converts the object to its String representation for persistence.
    /// Uses the ValueType's conversion logic.
    /// </summary>
    string ToString(object value) => ValueType.CastTo(value);

    /// <summary>
    /// Converts a String representation to the object, applying validation.
    /// Uses the ValueType's conversion logic and then validates the result.
    /// </summary>
    /// <exception cref="ArgumentException">if the value cannot be parsed or is invalid</exception>
    object FromString(string value)
    {
        object casted = ValueType.CastFrom(value);
        if (!IsValid(casted))
        {
            throw new ArgumentException($"Invalid value: '{value}'. Expected: {Desc}");
        }
        return casted;
    }
}
