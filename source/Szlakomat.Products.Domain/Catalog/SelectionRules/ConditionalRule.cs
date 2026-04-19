using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

public sealed record ConditionalRule(ISelectionRule Condition, List<ISelectionRule> ThenRules) : ISelectionRule
{
    public bool IsSatisfiedBy(List<SelectedProduct> selection) =>
        !Condition.IsSatisfiedBy(selection) || ThenRules.All(rule => rule.IsSatisfiedBy(selection));

    public override string ToString() =>
        $"ConditionalRule{{thenRules={ThenRules.Count}}}";
}
