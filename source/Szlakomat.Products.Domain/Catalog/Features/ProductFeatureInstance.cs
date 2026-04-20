namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// ProductFeatureInstance represents a specific feature (such as color) of a good or service
/// and its value (e.g., blue).
///
/// Examples:
/// - color: "red"
/// - size: "L"
/// - yearOfProduction: 2023
/// - expiryDate: 2024-12-31
///
/// Each ProductFeatureInstance:
/// - References its ProductFeatureType (which defines the constraint)
/// - Has a value that satisfies the type's constraint
/// - Can convert its value to/from String for persistence
///
/// The value type is validated against the feature type's constraint at construction time,
/// ensuring that invalid product instances cannot be created.
/// </summary>
internal class ProductFeatureInstance
{
    public ProductFeatureType FeatureType { get; }
    public object Value { get; }

    public ProductFeatureInstance(ProductFeatureType? featureType, object? value)
    {
        Guard.IsNotNull(featureType);
        Guard.IsNotNull(value);

        featureType.ValidateValue(value);

        FeatureType = featureType;
        Value = value;
    }

    public static ProductFeatureInstance Of(ProductFeatureType featureType, object value) => new(featureType, value);

    public static ProductFeatureInstance FromString(ProductFeatureType? featureType, string? stringValue)
    {
        Guard.IsNotNull(featureType);
        Guard.IsNotNull(stringValue);

        object parsedValue = featureType.Constraint.FromString(stringValue);
        return new ProductFeatureInstance(featureType, parsedValue);
    }

    public string ValueAsString() => FeatureType.Constraint.ToString(Value);

    public string AsString()
    {
        if (Value is not string stringValue)
            throw new InvalidOperationException($"Feature '{FeatureType.Name}' value is not a string (type: {Value.GetType().Name})");
        return stringValue;
    }

    public int AsInt()
    {
        if (Value is not int intValue)
            throw new InvalidOperationException($"Feature '{FeatureType.Name}' value is not an integer (type: {Value.GetType().Name})");
        return intValue;
    }

    public decimal AsDecimal()
    {
        if (Value is not decimal decimalValue)
            throw new InvalidOperationException($"Feature '{FeatureType.Name}' value is not a decimal (type: {Value.GetType().Name})");
        return decimalValue;
    }

    public DateOnly AsDate()
    {
        if (Value is not DateOnly dateValue)
            throw new InvalidOperationException($"Feature '{FeatureType.Name}' value is not a date (type: {Value.GetType().Name})");
        return dateValue;
    }

    public bool AsBoolean()
    {
        if (Value is not bool boolValue)
            throw new InvalidOperationException($"Feature '{FeatureType.Name}' value is not a boolean (type: {Value.GetType().Name})");
        return boolValue;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        ProductFeatureInstance that = (ProductFeatureInstance)obj;
        return Equals(FeatureType, that.FeatureType) && Equals(Value, that.Value);
    }

    public override int GetHashCode() => HashCode.Combine(FeatureType, Value);

    public override string ToString() => $"ProductFeatureInstance{{{FeatureType.Name}={Value}}}";

    public bool IsOfType(ProductFeatureType type) => FeatureType.Equals(type);
}
