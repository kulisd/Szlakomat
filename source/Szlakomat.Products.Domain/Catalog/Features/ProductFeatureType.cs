using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// ProductFeatureType reprezentuje typ cechy (takie jak kolor) towaru lub usługi
/// i definiuje ograniczenie na możliwe wartości.
///
/// Przykłady:
/// - "kolor" z dozwolonymi wartościami: {czerwony, niebieski, czarny, biały}
/// - "rozmiar" z dozwolonymi wartościami: {S, M, L, XL}
/// - "rokProdukcji" z zakresem numerycznym: 2020-2024
/// - "dataWażności" z zakresem daty: 2024-01-01 do 2024-12-31
///
/// Każdy ProductFeatureType definiuje:
/// - Unikalny identyfikator (nazwa)
/// - Ograniczenie, które definiuje typ wartości i reguły walidacji
///
/// Ten archetyp pozwala na elastyczną specyfikację cech produktu bez
/// konieczności tworzenia nowych atrybutów lub podklas dla każdego typu cechy.
/// </summary>
internal class ProductFeatureType
{
    public string Name { get; }
    public IFeatureValueConstraint Constraint { get; }

    internal ProductFeatureType(string? name, IFeatureValueConstraint? constraint)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(constraint);
        Name = name;
        Constraint = constraint;
    }

    public static ProductFeatureType WithAllowedValues(string name, params string[] allowedValues) =>
        new(name, AllowedValuesConstraint.Of(allowedValues));

    public static ProductFeatureType WithNumericRange(string name, int min, int max) =>
        new(name, new NumericRangeConstraint(min, max));

    public static ProductFeatureType WithDecimalRange(string name, string min, string max) =>
        new(name, DecimalRangeConstraint.Of(min, max));

    public static ProductFeatureType WithRegex(string name, string pattern) => new(name, new RegexConstraint(pattern));

    public static ProductFeatureType WithDateRange(string name, string from, string to) =>
        new(name, DateRangeConstraint.Between(from, to));

    public static ProductFeatureType Unconstrained(string name, FeatureValueType valueType) =>
        new(name, new FeatureConstraints.Unconstrained(valueType));

    public static ProductFeatureType Of(string name, IFeatureValueConstraint constraint) => new(name, constraint);

    public bool IsValidValue(object value) => Constraint.IsValid(value);

    public void ValidateValue(object? value)
    {
        Guard.IsNotNull(value);
        Guard.IsTrue(Constraint.ValueType.IsInstance(value),
            $"Feature '{Name}' expects type {Constraint.ValueType} but got {value.GetType().Name}");
        Guard.IsTrue(IsValidValue(value),
            $"Invalid value '{value}' for feature '{Name}'. Expected: {Constraint.Desc}");
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        ProductFeatureType that = (ProductFeatureType)obj;
        return Name == that.Name;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => $"ProductFeatureType{{name='{Name}', constraint={Constraint.Desc}}}";
}
