using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

// Metody fabrykacyjne
public static class SelectionRule
{
    public static ISelectionRule IsSubsetOf(ProductSet? sourceSet, int min, int max)
    {
        Guard.IsNotNull(sourceSet);
        Guard.IsGreaterThanOrEqualTo(min, 0);
        Guard.IsGreaterThanOrEqualTo(max, min);
        return new IsSubsetOfRule(sourceSet, min, max);
    }

    public static ISelectionRule Single(ProductSet sourceSet) => IsSubsetOf(sourceSet, 1, 1);

    public static ISelectionRule Optional(ProductSet sourceSet) => IsSubsetOf(sourceSet, 0, 1);

    public static ISelectionRule Required(ProductSet sourceSet) => IsSubsetOf(sourceSet, 1, int.MaxValue);

    public static ISelectionRule And(params ISelectionRule[]? rules)
    {
        Guard.IsNotEmpty(rules);
        return new AndRule(rules!.ToList());
    }

    public static ISelectionRule Or(params ISelectionRule[]? rules)
    {
        Guard.IsNotEmpty(rules);
        return new OrRule(rules!.ToList());
    }

    public static ISelectionRule IfThen(ISelectionRule? condition, params ISelectionRule[]? thenRules)
    {
        Guard.IsNotNull(condition);
        Guard.IsNotEmpty(thenRules);
        return new ConditionalRule(condition, thenRules!.ToList());
    }

    public static ISelectionRule Not(ISelectionRule? rule)
    {
        Guard.IsNotNull(rule);
        return new NotRule(rule);
    }
}
