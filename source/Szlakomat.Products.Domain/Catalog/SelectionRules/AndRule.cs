using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

public sealed record AndRule(List<ISelectionRule> Rules) : ISelectionRule
{
    public bool IsSatisfiedBy(List<SelectedProduct> selection) =>
        Rules.All(rule => rule.IsSatisfiedBy(selection));

    public override string ToString() =>
        $"AndRule{{rules={Rules.Count}}}";
}
