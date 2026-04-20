using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Ogranicza wartości tekstowe do predefiniowanego zestawu dozwolonych wartości.
/// Przykład: kolor może być jednym z {czerwony, niebieski, zielony}
///
/// Przykład konfiguracji trwałości: {"allowedValues": ["red", "blue", "green"]}
/// </summary>
internal class AllowedValuesConstraint : IFeatureValueConstraint
{
    private readonly IReadOnlySet<string> _allowedValues;

    public AllowedValuesConstraint(IReadOnlySet<string> allowedValues)
    {
        Guard.IsNotEmpty(allowedValues);
        _allowedValues = allowedValues;
    }

    public static AllowedValuesConstraint Of(params string[]? values)
    {
        Guard.IsNotEmpty(values);
        return new AllowedValuesConstraint(new HashSet<string>(values!));
    }

    public FeatureValueType ValueType => FeatureValueType.Text;

    public string Type => "ALLOWED_VALUES";

    public bool IsValid(object value) =>
        value is string stringValue && _allowedValues.Contains(stringValue);

    public string Desc => $"one of: {{{string.Join(", ", _allowedValues)}}}";

    public IReadOnlySet<string> GetAllowedValues() => _allowedValues;
}
