namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła Optional: zero lub jeden produkt z zestawu.
/// Skrót dla IsSubsetOf(set, 0, 1).
/// </summary>
public record OptionalConfig(
    IReadOnlySet<string> ProductIds // Product identifiers in the set
) : ISelectionRuleConfig;
