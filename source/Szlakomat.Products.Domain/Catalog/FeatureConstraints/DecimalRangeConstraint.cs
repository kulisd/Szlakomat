using System.Globalization;
using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Ogranicza wartości dziesiętne do zakresu numerycznego [min, max].
/// Przykład: waga między 0,5 a 100,0 kg
///
/// Przykład konfiguracji trwałości: {"min": "0.5", "max": "100.0"}
/// </summary>
internal class DecimalRangeConstraint : IFeatureValueConstraint
{
    private readonly decimal _min;
    private readonly decimal _max;

    public DecimalRangeConstraint(decimal min, decimal max)
    {
        Guard.IsLessThanOrEqualTo(min, max);
        _min = min;
        _max = max;
    }

    public static DecimalRangeConstraint Of(string min, string max) =>
        new(
            decimal.Parse(min, CultureInfo.InvariantCulture),
            decimal.Parse(max, CultureInfo.InvariantCulture));

    public static IFeatureValueConstraint Between(decimal min, decimal max) =>
        new DecimalRangeConstraint(min, max);

    public FeatureValueType ValueType => FeatureValueType.Decimal;

    public string Type => "DECIMAL_RANGE";

    public bool IsValid(object value)
    {
        if (value is not decimal decimalValue)
            return false;
        return decimalValue >= _min && decimalValue <= _max;
    }

    public string Desc => $"decimal between {_min} and {_max}";

    public decimal Min => _min;

    public decimal Max => _max;
}
