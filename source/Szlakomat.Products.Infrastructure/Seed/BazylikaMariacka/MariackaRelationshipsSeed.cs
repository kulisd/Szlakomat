using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.BazylikaMariacka;

/// <summary>
/// Seeds ProductRelationship instances for Bazylika Mariacka.
///
/// Key design decision: wnętrze and Hejnalica are TWO INDEPENDENT ROUTES.
/// Choosing one does NOT exclude the other — they have different hours, prices,
/// and restrictions. Therefore we model them as CompatibleWith (bidirectional),
/// NOT as SubstitutedBy or any exclusive relationship.
///
/// CompatibleWith means: "these two products work well together and visitors
/// can purchase both on the same day".
/// </summary>
internal static class MariackaRelationshipsSeed
{
    public static List<ProductRelationship> Seed(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        ProductType basilikaInterior,
        ProductType hejnalica)
    {
        var relationships = new List<ProductRelationship>();

        // Interior and Hejnalica are COMPATIBLE — not exclusive.
        // A visitor can do both on the same visit day:
        //   - First enter the basilica interior (11:30+ on weekdays, 14:00+ on Sundays)
        //   - Then queue for the tower (Fri–Sun only, last entry 17:30)
        // Or visit the tower first and the interior after.
        Define(factory, relationships, basilikaInterior, hejnalica,
            ProductRelationshipType.CompatibleWith);

        Define(factory, relationships, hejnalica, basilikaInterior,
            ProductRelationshipType.CompatibleWith);

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
