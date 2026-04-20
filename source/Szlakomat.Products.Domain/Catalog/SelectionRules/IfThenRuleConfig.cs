namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// Reguła IF-THEN: jeśli warunek jest spełniony, to wszystkie reguły 'then' muszą być spełnione.
/// </summary>
public record IfThenRuleConfig(
    ISelectionRuleConfig Condition,
    IReadOnlySet<ISelectionRuleConfig> ThenRules
) : ISelectionRuleConfig;
