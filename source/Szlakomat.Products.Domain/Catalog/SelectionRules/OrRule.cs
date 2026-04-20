using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

public sealed record OrRule(List<ISelectionRule> Rules) : ISelectionRule
{
    public bool IsSatisfiedBy(List<SelectedProduct> selection) =>
        Rules.Any(rule => rule.IsSatisfiedBy(selection));

    public override string ToString() =>
        $"OrRule{{rules={Rules.Count}}}";
}
