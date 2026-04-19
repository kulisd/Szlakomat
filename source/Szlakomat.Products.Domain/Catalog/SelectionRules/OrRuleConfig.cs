namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła OR: przynajmniej jedna zagnieżdżona reguła musi być spełniona.
/// </summary>
public record OrRuleConfig(
    IReadOnlySet<ISelectionRuleConfig> Rules
) : ISelectionRuleConfig;
