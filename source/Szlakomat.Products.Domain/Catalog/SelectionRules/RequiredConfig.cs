namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła Required: przynajmniej jeden produkt z zestawu.
/// Skrót dla IsSubsetOf(set, 1, int.MaxValue).
/// </summary>
public record RequiredConfig(
    IReadOnlySet<string> ProductIds // Product identifiers in the set
) : ISelectionRuleConfig;
