using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

public sealed record IsSubsetOfRule(ProductSet SourceSet, int Min, int Max) : ISelectionRule
{
    public bool IsSatisfiedBy(List<SelectedProduct> selection)
    {
        int count = selection
            .Where(s => SourceSet.Contains(s.ProductId))
            .Sum(s => s.Quantity);
        return count >= Min && count <= Max;
    }

    public override string ToString() =>
        $"IsSubsetOf{{set='{SourceSet.Name}', min={Min}, max={Max}}}";
}
