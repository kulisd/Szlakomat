using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Catalog.PackageType;

/// <summary>
/// ProductSet reprezentuje nazwany zbiór produktów dostępnych do wyboru w pakiecie.
/// To jest "surowiec" - pula opcji produktowych bez żadnych ograniczeń wyboru.
///
/// Przykład: Pakiet laptopa może mieć ProductSets takie jak "Opcje pamięci" (4GB, 8GB, 16GB),
/// "Opcje przechowywania" (256GB SSD, 512GB SSD, 1TB SSD), "Akcesoria" (mysz, klawiatura, torba).
/// </summary>
public class ProductSet
{
    public string Name { get; }
    public IReadOnlySet<IProductIdentifier> Products { get; }

    public ProductSet(string? name, IEnumerable<IProductIdentifier>? products)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(products);
        Guard.IsTrue(products.Any());
        Name = name;
        Products = new HashSet<IProductIdentifier>(products);
    }

    public static ProductSet SingleOf(string name, IProductIdentifier id) => new(name, new[] { id });

    public static ProductSet Of(string name, params IProductIdentifier[] ids) => new(name, ids);

    public bool Contains(IProductIdentifier productId) => Products.Contains(productId);

    public override string ToString() =>
        $"ProductSet{{name='{Name}', products={Products.Count}}}";
}
