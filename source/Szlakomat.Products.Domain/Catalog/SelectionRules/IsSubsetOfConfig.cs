namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła IsSubsetOf: wybierz od min do max produktów z zestawu.
/// Przykład: Wybierz 2-3 akcesoria z {mysz, klawiatura, monitor}.
/// </summary>
public record IsSubsetOfConfig(
    IReadOnlySet<string> ProductIds, // Product identifiers in the set
    int Min,
    int Max
) : ISelectionRuleConfig;
