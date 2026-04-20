using System.Text.RegularExpressions;
using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Waliduje wartości tekstowe względem wzorca wyrażenia regularnego.
/// Przykład: kod produktu musi odpowiadać wzorcowi "^[A-Z]{2}-\d{4}$"
///
/// Przykład konfiguracji trwałości: {"pattern": "^[A-Z]{2}-\\d{4}$"}
/// </summary>
internal class RegexConstraint : IFeatureValueConstraint
{
    private readonly Regex _pattern;
    private readonly string _patternString;

    public RegexConstraint(string? pattern)
    {
        Guard.IsNotNullOrWhiteSpace(pattern);
        _patternString = pattern;
        _pattern = new Regex(pattern);
    }

    public static IFeatureValueConstraint Of(string pattern) =>
        new RegexConstraint(pattern);

    public FeatureValueType ValueType => FeatureValueType.Text;

    public string Type => "REGEX";

    public bool IsValid(object value)
    {
        if (value is not string stringValue)
            return false;
        return _pattern.IsMatch(stringValue);
    }

    public string Desc => $"text matching pattern: {_patternString}";

    public string Pattern => _patternString;
}
