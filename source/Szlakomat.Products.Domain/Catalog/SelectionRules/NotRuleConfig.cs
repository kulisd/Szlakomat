namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła NOT: odwraca wynik zagnieżdżonej reguły.
/// </summary>
public record NotRuleConfig(
    ISelectionRuleConfig Rule
) : ISelectionRuleConfig;
