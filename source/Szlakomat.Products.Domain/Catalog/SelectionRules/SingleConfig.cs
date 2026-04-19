namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła Single: dokładnie jeden produkt z zestawu.
/// Skrót dla IsSubsetOf(set, 1, 1).
/// </summary>
public record SingleConfig(
    IReadOnlySet<string> ProductIds // Product identifiers in the set
) : ISelectionRuleConfig;
