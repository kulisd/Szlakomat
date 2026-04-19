using System.Globalization;

namespace Szlakomat.Products.Domain.Catalog.Features;

public static class FeatureValueTypeExtensions
{
    /// <summary>
    /// Converts a String representation to the runtime type.
    /// </summary>
    /// <exception cref="ArgumentException">if the value cannot be parsed</exception>
    public static object CastFrom(this FeatureValueType type, string value) => type switch
    {
        FeatureValueType.Text => value,
        FeatureValueType.Integer => int.Parse(value, CultureInfo.InvariantCulture),
        FeatureValueType.Decimal => decimal.Parse(value, CultureInfo.InvariantCulture),
        FeatureValueType.Date => DateOnly.Parse(value, CultureInfo.InvariantCulture),
        FeatureValueType.Boolean => bool.Parse(value),
        _ => throw new ArgumentException($"Unknown type: {type}")
    };

    /// <summary>
    /// Converts the runtime type to its String representation (for persistence).
    /// </summary>
    public static string CastTo(this FeatureValueType type, object value) => type switch
    {
        FeatureValueType.Text => (string)value,
        FeatureValueType.Integer => ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture),
        FeatureValueType.Decimal => ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture),
        FeatureValueType.Date => ((IFormattable)value).ToString("O", CultureInfo.InvariantCulture),
        FeatureValueType.Boolean => value.ToString() ?? "",
        _ => throw new ArgumentException($"Unknown type: {type}")
    };

    /// <summary>
    /// Returns the Type that represents this value type.
    /// </summary>
    public static Type GetValueType(this FeatureValueType type) => type switch
    {
        FeatureValueType.Text => typeof(string),
        FeatureValueType.Integer => typeof(int),
        FeatureValueType.Decimal => typeof(decimal),
        FeatureValueType.Date => typeof(DateOnly),
        FeatureValueType.Boolean => typeof(bool),
        _ => throw new ArgumentException($"Unknown type: {type}")
    };

    /// <summary>
    /// Checks if the given value is an instance of this type.
    /// </summary>
    public static bool IsInstance(this FeatureValueType type, object value) =>
        GetValueType(type).IsInstanceOfType(value);
}
