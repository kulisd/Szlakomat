using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Ogranicza wartości daty do zakresu daty [od, do].
/// Przykład: data wygaśnięcia między 2024-01-01 a 2024-12-31
///
/// Przykład konfiguracji trwałości: {"from": "2024-01-01", "to": "2024-12-31"}
/// </summary>
internal class DateRangeConstraint : IFeatureValueConstraint
{
    private readonly DateOnly _from;
    private readonly DateOnly _to;

    public DateRangeConstraint(DateOnly from, DateOnly to)
    {
        Guard.IsLessThanOrEqualTo(from, to);
        _from = from;
        _to = to;
    }

    public static DateRangeConstraint Between(string from, string to) => new(DateOnly.Parse(from), DateOnly.Parse(to));

    public FeatureValueType ValueType => FeatureValueType.Date;

    public string Type => "DATE_RANGE";

    public bool IsValid(object value)
    {
        if (value is not DateOnly dateValue)
            return false;
        return dateValue >= _from && dateValue <= _to;
    }

    public string Desc => $"date between {_from} and {_to}";

    public DateOnly From => _from;

    public DateOnly To => _to;
}
