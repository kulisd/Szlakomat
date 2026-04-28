using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.ParkWodny;

/// <summary>
/// Seeds ProductRelationship instances between Park Wodny Kraków products:
/// — komplementarne strefy i usługi
/// — ścieżki ulepszenia biletu
/// — substytucje (sauna ↔ masaż jako alternatywy relaksacyjne)
/// — kompatybilne kombinacje wizyt
/// </summary>
internal static class ParkWodnyRelationshipsSeed
{
    public static List<ProductRelationship> Seed(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        List<ProductType> attractions,
        List<ProductType> services,
        List<PackageType> packages)
    {
        var attr = attractions.ToDictionary(a => a.Name().Value);
        var svc = services.ToDictionary(s => s.Name().Value);
        var pkg = packages.ToDictionary(p => p.Name().Value);

        var basenRek = attr["Basen Rekreacyjny"];
        var basenSport = attr["Basen Sportowy"];
        var zjezdzalnie = attr["Zjeżdżalnie Wodne"];
        var aquaKids = attr["Aqua Kids — Strefa dla Dzieci"];
        var rzekaJacuzzi = attr["Rzeka Leniwca i Jacuzzi"];
        var strefaSaun = attr["Strefa Saun"];
        var basenZew = attr["Basen Zewnętrzny"];
        var masazKlas = attr["Masaż Klasyczny (50 min)"];
        var masazKamienie = attr["Masaż Gorącymi Kamieniami (60 min)"];
        var aquaAerobik = attr["Aqua Aerobik — zajęcia grupowe"];
        var naukaPlyDzieci = attr["Nauka Pływania — Dzieci (4–12 lat)"];
        var naukaPlyDoroslych = attr["Nauka Pływania — Dorośli"];
        var recznik = svc["Wypożyczenie ręcznika"];

        var biletAqua = pkg["Bilet Aqua Park"];
        var biletAquaSpa = pkg["Bilet Aqua Park + SPA"];
        var biletRodzinny = pkg["Bilet Rodzinny (2+2)"];
        var biletGrupowy = pkg["Bilet Grupowy (min. 10 osób)"];
        var karnetAqua = pkg["Karnet Miesięczny — Aqua Park"];
        var karnetAquaSpa = pkg["Karnet Miesięczny — Aqua Park + SPA"];
        var wellness = pkg["Wizyta Wellness — SPA + Masaż"];

        var relationships = new List<ProductRelationship>();

        // ── Komplementarne atrakcje wodne ────────────────────────────────
        // Basen Rekreacyjny i Rzeka Leniwca razem tworzą pełen relaks
        Define(factory, relationships, basenRek, rzekaJacuzzi, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, rzekaJacuzzi, basenRek, ProductRelationshipType.ComplementedBy);

        // Zjeżdżalnie dobrze łączą się z basenem rekreacyjnym
        Define(factory, relationships, zjezdzalnie, basenRek, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, basenRek, zjezdzalnie, ProductRelationshipType.ComplementedBy);

        // Aqua Kids komplementuje wizytę rodzica (basen sportowy lub rekreacyjny)
        Define(factory, relationships, aquaKids, basenRek, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, aquaKids, basenSport, ProductRelationshipType.CompatibleWith);

        // Basen zewnętrzny (letni) komplementuje strefę saun
        Define(factory, relationships, basenZew, strefaSaun, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, strefaSaun, basenZew, ProductRelationshipType.ComplementedBy);

        // ── Substytucje ──────────────────────────────────────────────────
        // Masaż klasyczny i gorące kamienie są alternatywami relaksacyjnymi
        Define(factory, relationships, masazKlas, masazKamienie, ProductRelationshipType.SubstitutedBy);
        Define(factory, relationships, masazKamienie, masazKlas, ProductRelationshipType.SubstitutedBy);

        // Aqua Aerobik i Nauka Pływania (dorośli) jako alternatywne aktywności
        Define(factory, relationships, aquaAerobik, naukaPlyDoroslych, ProductRelationshipType.SubstitutedBy);

        // ── Kompatybilne kombinacje wizyt ────────────────────────────────
        // Basen sportowy i aqua aerobik — dla aktywnych
        Define(factory, relationships, basenSport, aquaAerobik, ProductRelationshipType.CompatibleWith);

        // Nauka pływania dzieci + Aqua Kids — pełna wizyta dla rodziny z małymi dziećmi
        Define(factory, relationships, naukaPlyDzieci, aquaKids, ProductRelationshipType.CompatibleWith);

        // Strefa saun dobrze łączy się z masażem
        Define(factory, relationships, strefaSaun, masazKlas, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, strefaSaun, masazKamienie, ProductRelationshipType.ComplementedBy);

        // Ręcznik komplementuje każdą wizytę w SPA
        Define(factory, relationships, strefaSaun, recznik, ProductRelationshipType.ComplementedBy);

        // ── Ścieżki ulepszenia biletu ────────────────────────────────────
        // Bilet Aqua Park → Bilet Aqua Park + SPA
        Define(factory, relationships, biletAqua, biletAquaSpa, ProductRelationshipType.UpgradableTo);

        // Bilet Aqua Park → Karnet Miesięczny (długoterminowe oszczędności)
        Define(factory, relationships, biletAqua, karnetAqua, ProductRelationshipType.UpgradableTo);

        // Bilet Aqua Park + SPA → Karnet Miesięczny Aqua + SPA
        Define(factory, relationships, biletAquaSpa, karnetAquaSpa, ProductRelationshipType.UpgradableTo);

        // Bilet Grupowy → Bilet Rodzinny (mniejsza grupa z dziećmi)
        Define(factory, relationships, biletGrupowy, biletRodzinny, ProductRelationshipType.SubstitutedBy);

        // Bilet Aqua Park + SPA → Wizyta Wellness (upgrade do pełnego doznania SPA)
        Define(factory, relationships, biletAquaSpa, wellness, ProductRelationshipType.UpgradableTo);

        // Karnet Aqua → Karnet Aqua + SPA
        Define(factory, relationships, karnetAqua, karnetAquaSpa, ProductRelationshipType.UpgradableTo);

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
