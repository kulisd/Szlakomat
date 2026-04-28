using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.KopiecKosciuszki;

/// <summary>
/// Seeds ProductRelationship instances for Kopiec Kościuszki.
/// Obejmuje: relacje uzupełniające między atrakcjami, ścieżki upgrade między pakietami,
/// substytuty usług oraz kompatybilne wizyty.
/// </summary>
internal static class KopiecKosciuszkiRelationshipsSeed
{
    public static List<ProductRelationship> Seed(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        List<ProductType> attractions,
        List<ProductType> services,
        List<PackageType> packages)
    {
        var attraction = attractions.ToDictionary(a => a.Name().Value);
        var service = services.ToDictionary(s => s.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);

        var kopiec = attraction["Wejście na Kopiec Kościuszki"];
        var muzeum = attraction["Muzeum im. Tadeusza Kościuszki"];
        var galeria = attraction["Galeria Sztuki Patriotycznej"];
        var multimedia = attraction["Wystawa multimedialna „Kościuszko — Człowiek i Symbol""];
        var fortTrail = attraction["Trasa Fortyfikacyjna Fortu Kościuszki"];

        var przewodnikPl = service["Przewodnik — język polski"];
        var przewodnikObcy = service["Przewodnik — język obcy"];
        var audioprzewodnik = service["Audioprzewodnik Kopiec Kościuszki"];

        var biletKopiec = package["Bilet wstępu na Kopiec Kościuszki"];
        var biletKopiecMuzeum = package["Bilet łączony: Kopiec + Muzeum Kościuszki"];
        var biletKompleksowy = package["Bilet kompleksowy — Kopiec i Fort"];

        var relationships = new List<ProductRelationship>();

        // Muzeum i galeria wzajemnie się uzupełniają (obie w forcie)
        Define(factory, relationships, muzeum, galeria, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, galeria, muzeum, ProductRelationshipType.ComplementedBy);

        // Wystawa multimedialna uzupełnia muzeum (nowoczesna vs. tradycyjna)
        Define(factory, relationships, muzeum, multimedia, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, multimedia, muzeum, ProductRelationshipType.ComplementedBy);

        // Trasa fortyfikacyjna uzupełnia wszystkie atrakcje wewnątrz fortu
        Define(factory, relationships, fortTrail, muzeum, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, fortTrail, galeria, ProductRelationshipType.ComplementedBy);
        Define(factory, relationships, fortTrail, multimedia, ProductRelationshipType.ComplementedBy);

        // Wejście na kopiec jest kompatybilne z każdą atrakcją w forcie
        Define(factory, relationships, kopiec, muzeum, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, kopiec, multimedia, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, kopiec, galeria, ProductRelationshipType.CompatibleWith);
        Define(factory, relationships, kopiec, fortTrail, ProductRelationshipType.CompatibleWith);

        // Audioprzewodnik jako substytut przewodnika
        Define(factory, relationships, audioprzewodnik, przewodnikPl, ProductRelationshipType.SubstitutedBy);
        Define(factory, relationships, audioprzewodnik, przewodnikObcy, ProductRelationshipType.SubstitutedBy);

        // Ścieżki upgrade między pakietami
        Define(factory, relationships, biletKopiec, biletKopiecMuzeum, ProductRelationshipType.UpgradableTo);
        Define(factory, relationships, biletKopiec, biletKompleksowy, ProductRelationshipType.UpgradableTo);
        Define(factory, relationships, biletKopiecMuzeum, biletKompleksowy, ProductRelationshipType.UpgradableTo);

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
