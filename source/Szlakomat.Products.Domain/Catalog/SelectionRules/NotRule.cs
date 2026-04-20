using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

public sealed record NotRule(ISelectionRule Rule) : ISelectionRule
{
    public bool IsSatisfiedBy(List<SelectedProduct> selection) =>
        !Rule.IsSatisfiedBy(selection);

    public override string ToString() =>
        $"NotRule{{rule={Rule}}}";
}
