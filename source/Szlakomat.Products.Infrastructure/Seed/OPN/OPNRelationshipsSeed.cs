using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.OPN;

/// <summary>
/// Seeds ProductRelationship instances between OPN products:
/// complementary attractions, trail-attraction connections, and package upgrade paths.
/// </summary>
internal static class OPNRelationshipsSeed
{
    public static List<ProductRelationship> Seed(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        List<ProductType> attractions,
        List<ProductType> trails,
        List<PackageType> packages)
    {
        var attraction = attractions.ToDictionary(a => a.Name().Value);
        var trail = trails.ToDictionary(t => t.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);

        var lokietka = attraction["Jaskinia Łokietka"];
        var ciemna = attraction["Jaskinia Ciemna"];
        var zamekOjcow = attraction["Zamek Kazimierzowski w Ojcowie"];
        var zamekPieskowa = attraction["Zamek w Pieskowej Skale"];
        var bramaKrakowska = attraction["Brama Krakowska, Jonaszówka i Źródło Miłości"];
        var maczuga = attraction["Maczuga Herkulesa"];

        var czerwony = trail["Szlak Orlich Gniazd (czerwony)"];
        var niebieski = trail["Szlak Warowni Jurajskich (niebieski)"];
        var zielony = trail["Szlak Park Zamkowy – Jaskinia Ciemna (zielony)"];
        var czarny = trail["Szlak Łącznik Widokowy (czarny)"];

        var pkgJaskinie = package["OPN — Jaskinie"];
        var pkgZamki = package["OPN — Zamki Jurajskie"];
        var pkgOdkryj = package["OPN — Odkryj Park"];

        var relationships = new List<ProductRelationship>();

        // Jaskinie wzajemnie się uzupełniają — naturalna para
        Define(factory, relationships, lokietka, ciemna, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, ciemna, lokietka, ProductRelationshipType.ComplementedBy);

        // Zamki wzajemnie się uzupełniają — dwie epoki, ta sama trasa
        Define(factory, relationships, zamekOjcow, zamekPieskowa, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, zamekPieskowa, zamekOjcow, ProductRelationshipType.ComplementedBy);

        // Jaskinia Ciemna — dojście szlakiem zielonym
        Define(factory, relationships, ciemna, zielony, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, zielony, ciemna, ProductRelationshipType.CompatibleWith);

        // Jaskinia Łokietka — dojście szlakiem niebieskim
        Define(factory, relationships, lokietka, niebieski, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, niebieski, lokietka, ProductRelationshipType.CompatibleWith);

        // Brama Krakowska leży przy szlaku czerwonym i bezpośrednio pod Jaskinią Ciemną
        Define(factory, relationships, bramaKrakowska, czerwony, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, bramaKrakowska, ciemna, ProductRelationshipType.CompatibleWith);

        // Maczuga Herkulesa widoczna ze szlaku czarnego, przy Zamku Pieskowa Skała
        Define(factory, relationships, maczuga, czarny, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, maczuga, zamekPieskowa, ProductRelationshipType.ComplementedBy);

        // Szlak czerwony — główna oś łącząca wszystkie atrakcje
        Define(factory, relationships, czerwony, bramaKrakowska, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, czerwony, zamekOjcow, ProductRelationshipType.CompatibleWith);

        // Ścieżki pakietów — upgrade do pełniejszego zwiedzania
        Define(factory, relationships, pkgJaskinie, pkgOdkryj, ProductRelationshipType.UpgradableTo);
        Define(factory, relationships, pkgZamki, pkgOdkryj, ProductRelationshipType.UpgradableTo);

        foreach (var relationship in relationships)
        {
            repo.Save(relationship);
        }

        return relationships;
    }

    private static void Define(
        ProductRelationshipFactory factory,
        List<ProductRelationship> relationships,
        IProduct from,
        IProduct to,
        ProductRelationshipType type)
    {
        var result = factory.DefineFor(from.Id(), to.Id(), type);
        var relationship = result.GetSuccess();
        if (relationship != null)
        {
            relationships.Add(relationship);
        }
    }
}
