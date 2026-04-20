using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.FeatureConstraints;

/// <summary>
/// Brak ograniczeń - każda wartość określonego typu jest ważna.
/// Przykład: dowolny tekst dla pola komentarza o swobodnej formie
///
/// Przykład konfiguracji trwałości: {} (puste)
/// </summary>
internal class Unconstrained : IFeatureValueConstraint
{
    private readonly FeatureValueType _valueType;

    public Unconstrained(FeatureValueType valueType)
    {
        Guard.IsNotNull(valueType);
        _valueType = valueType;
    }

    public FeatureValueType ValueType => _valueType;

    public string Type => "UNCONSTRAINED";

    public bool IsValid(object value) => _valueType.IsInstance(value);

    public string Desc => $"any {_valueType.ToString().ToLowerInvariant()}";
}
