using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.SelectionRules;

/// <summary>
/// SelectionRule defines constraints about how products can be selected in a package.
/// While ProductSet defines WHAT products are available, SelectionRule defines
/// HOW MANY can be selected (constraints).
///
/// Supports composition through AND, OR, and conditional (IF-THEN) logic.
/// </summary>
public interface ISelectionRule
{
    bool IsSatisfiedBy(List<SelectedProduct> selection);
}
