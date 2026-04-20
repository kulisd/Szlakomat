using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.SelectionRules;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Tests.Domain;

public class PackageStructureTests
{
    private static readonly IProductIdentifier A = UuidProductIdentifier.New();
    private static readonly IProductIdentifier B = UuidProductIdentifier.New();
    private static readonly IProductIdentifier C = UuidProductIdentifier.New();

    private static PackageStructure BuildStructure(int min, int max, params IProductIdentifier[] available)
    {
        var set = ProductSet.Of("set", available);
        var rule = SelectionRule.IsSubsetOf(set, min, max);
        var sets = new Dictionary<string, ProductSet> { ["set"] = set };
        var rules = new List<ISelectionRule> { rule };
        return new PackageStructure(sets, rules);
    }

    [Fact]
    public void WithChoice_RequiresExactlyN_AllNSelected_Succeeds()
    {
        var structure = BuildStructure(min: 2, max: 2, A, B);
        var selection = new List<SelectedProduct>
        {
            new(A, 1),
            new(B, 1)
        };

        var result = structure.Validate(selection);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void WithChoice_RequiresExactlyN_OnlyOneSelected_Fails()
    {
        var structure = BuildStructure(min: 2, max: 2, A, B);
        var selection = new List<SelectedProduct> { new(A, 1) };

        var result = structure.Validate(selection);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void WithChoice_RequiresExactlyN_MoreThanNSelected_Fails()
    {
        var structure = BuildStructure(min: 2, max: 2, A, B, C);
        var selection = new List<SelectedProduct>
        {
            new(A, 1),
            new(B, 1),
            new(C, 1)
        };

        var result = structure.Validate(selection);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void WithRequiredChoice_AtLeastOneSelected_Succeeds()
    {
        var structure = BuildStructure(min: 1, max: int.MaxValue, A, B);
        var selection = new List<SelectedProduct> { new(A, 1) };

        var result = structure.Validate(selection);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void WithRequiredChoice_NothingSelected_Fails()
    {
        var structure = BuildStructure(min: 1, max: int.MaxValue, A, B);
        var selection = new List<SelectedProduct>();

        var result = structure.Validate(selection);

        result.IsValid.Should().BeFalse();
    }
}
