using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Ogranicza wartości całkowite do zakresu numerycznego [min, max].
/// Przykład: rok produkcji między 2020 a 2024
///
/// Przykład konfiguracji trwałości: {"min": 2020, "max": 2024}
/// </summary>
internal class NumericRangeConstraint : IFeatureValueConstraint
{
    private readonly int _min;
    private readonly int _max;

    public NumericRangeConstraint(int min, int max)
    {
        Guard.IsLessThanOrEqualTo(min, max);
        _min = min;
        _max = max;
    }

    public static IFeatureValueConstraint Between(int min, int max) =>
        new NumericRangeConstraint(min, max);

    public FeatureValueType ValueType => FeatureValueType.Integer;

    public string Type => "NUMERIC_RANGE";

    public bool IsValid(object value)
    {
        if (value is not int intValue)
            return false;
        return intValue >= _min && intValue <= _max;
    }

    public string Desc => $"integer between {_min} and {_max}";

    public int Min => _min;

    public int Max => _max;
}
