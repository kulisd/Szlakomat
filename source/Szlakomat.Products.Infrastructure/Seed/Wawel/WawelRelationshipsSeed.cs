using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.Wawel;

/// <summary>
/// Seeds ProductRelationship instances between Wawel products:
/// complementary exhibitions, substitute services, upgrade paths, and compatible visits.
/// </summary>
internal static class WawelRelationshipsSeed
{
    public static List<ProductRelationship> Seed(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        List<ProductType> exhibitions,
        List<ProductType> services,
        List<PackageType> packages)
    {
        var exhibition = exhibitions.ToDictionary(e => e.Name().Value);
        var service = services.ToDictionary(s => s.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);

        var apartamenty = exhibition["Prywatne Apartamenty Królewskie"];
        var komnaty = exhibition["Reprezentacyjne Komnaty Królewskie"];
        var skarbiec = exhibition["Skarbiec Koronny"];
        var zbrojownia = exhibition["Zbrojownia"];
        var podziemia = exhibition["Podziemia Zamku"];
        var miedzymurze = exhibition["Międzymurze. Podziemia Wawelu"];
        var ogrody = exhibition["Ogrody Królewskie"];
        var wawelOdzyskany = exhibition["Wawel Odzyskany"];
        var smoczaJama = exhibition["Smocza Jama"];
        var basztaWidokowa = exhibition["Baszta Widokowa"];

        var przewodnikPl = service["Przewodnik licencjonowany — język polski"];
        var przewodnikObcy = service["Przewodnik licencjonowany — język obcy"];
        var audioprzewodnik = service["Audioprzewodnik"];
        var sluchawki = service["Wypożyczenie słuchawek grupowych"];

        var wizytaIndywidualna = package["Zamek Królewski na Wawelu — Wizyta indywidualna"];
        var wycieczkaGrupowa = package["Zamek Królewski na Wawelu — Wycieczka grupowa z przewodnikiem"];
        var zamekCombined = package["Zamek (piętro 1+2)"];
        var biletRoczny = package["Bilet roczny"];

        var relationships = new List<ProductRelationship>();

        // Complementary exhibitions — castle floors
        Define(factory, relationships, apartamenty, komnaty, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, komnaty, apartamenty, ProductRelationshipType.ComplementedBy);

        // Complementary exhibitions — Skarbiec + Zbrojownia
        Define(factory, relationships, skarbiec, zbrojownia, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, zbrojownia, skarbiec, ProductRelationshipType.ComplementedBy);

        // Audio guide as substitute for personal guide
        Define(factory, relationships, audioprzewodnik, przewodnikPl, ProductRelationshipType.SubstitutedBy);
        Define(factory, relationships, audioprzewodnik, przewodnikObcy, ProductRelationshipType.SubstitutedBy);

        // Upgrade path: individual → group
        Define(factory, relationships, wizytaIndywidualna, wycieczkaGrupowa, ProductRelationshipType.UpgradableTo);

        // Upgrade path: individual → combined castle ticket
        Define(factory, relationships, wizytaIndywidualna, zamekCombined, ProductRelationshipType.UpgradableTo);

        // Upgrade path: individual → annual pass
        Define(factory, relationships, wizytaIndywidualna, biletRoczny, ProductRelationshipType.UpgradableTo);

        // Guide complemented by headsets
        Define(factory, relationships, przewodnikPl, sluchawki, ProductRelationshipType.ComplementedBy);

        // Compatible visit combinations
        Define(factory, relationships, apartamenty, podziemia, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, miedzymurze, podziemia, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, ogrody, podziemia, ProductRelationshipType.CompatibleWith);

        // Wawel Odzyskany requires castle ticket
        Define(factory, relationships, wawelOdzyskany, apartamenty, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, wawelOdzyskany, komnaty, ProductRelationshipType.ComplementedBy);

        // Seasonal attractions — Szukając Smoka trail combines Międzymurze + Smocza Jama
        Define(factory, relationships, miedzymurze, smoczaJama, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, smoczaJama, miedzymurze, ProductRelationshipType.ComplementedBy);

        // Baszta Widokowa complements outdoor walks
        Define(factory, relationships, basztaWidokowa, ogrody, ProductRelationshipType.CompatibleWith);

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
