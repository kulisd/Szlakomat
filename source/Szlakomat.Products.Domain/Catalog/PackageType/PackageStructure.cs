using Szlakomat.Products.Domain.Catalog.SelectionRules;

namespace Szlakomat.Products.Domain.Catalog.PackageType;

/// <summary>
/// PackageStructure defines what products can be in a package and how they can be combined.
///
/// It consists of two parts:
/// 1. ProductSets - collections of available products (the "raw material")
/// 2. SelectionRules - rules defining valid selections (the "constraints")
///
/// Think of ProductSets as ingredients available in a kitchen, and SelectionRules
/// as the recipe rules: "use 1 meat", "add 2-3 vegetables", "if chicken then must add sauce".
/// </summary>
internal class PackageStructure
{
    public IReadOnlyDictionary<string, ProductSet> ProductSets { get; }
    public IReadOnlyList<ISelectionRule> SelectionRules { get; }

    public PackageStructure(IReadOnlyDictionary<string, ProductSet>? productSets,
                            IReadOnlyList<ISelectionRule>? selectionRules)
    {
        Guard.IsNotEmpty(productSets);
        Guard.IsNotEmpty(selectionRules);
        ProductSets = productSets!;
        SelectionRules = selectionRules!;
    }

    public PackageValidationResult Validate(List<SelectedProduct> selection)
    {
        var errors = new List<string>();

        for (int i = 0; i < SelectionRules.Count; i++)
        {
            var rule = SelectionRules[i];
            if (!rule.IsSatisfiedBy(selection))
            {
                errors.Add($"Rule {i + 1} not satisfied: {rule}");
            }
        }

        return errors.Count == 0
            ? PackageValidationResult.Success()
            : PackageValidationResult.Failure(errors);
    }

    public override string ToString() =>
        $"PackageStructure{{sets={ProductSets.Count}, rules={SelectionRules.Count}}}";
}
