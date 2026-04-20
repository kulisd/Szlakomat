namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła AND: wszystkie zagnieżdżone reguły muszą być spełnione.
/// </summary>
public record AndRuleConfig(
    IReadOnlySet<ISelectionRuleConfig> Rules
) : ISelectionRuleConfig;
