using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.Seed.OPN;

/// <summary>
/// Seeds OPN package types:
/// 1. OPN — Jaskinie — Łokietka + Ciemna (guided caves)
/// 2. OPN — Zamki Jurajskie — Kazimierzowski + Pieskowa Skała
/// 3. OPN — Odkryj Park — choice of 1–4 paid attractions
/// </summary>
internal static class OPNPackagesSeed
{
    public static List<PackageType> Seed(
        IPackageTypeRepository repo,
        List<ProductType> attractions)
    {
        var lokietka = attractions.First(a => a.Name().Value == "Jaskinia Łokietka");
        var ciemna = attractions.First(a => a.Name().Value == "Jaskinia Ciemna");
        var zamekOjcow = attractions.First(a => a.Name().Value == "Zamek Kazimierzowski w Ojcowie");
        var zamekPieskowa = attractions.First(a => a.Name().Value == "Zamek w Pieskowej Skale");

        var paidAttractionIds = new[]
        {
            zamekOjcow.Id(),
            lokietka.Id(),
            ciemna.Id(),
            zamekPieskowa.Id(),
        };

        var packages = new List<PackageType>
        {
            BuildJaskinie(lokietka, ciemna),
            BuildZamkiJurajskie(zamekOjcow, zamekPieskowa),
            BuildOdkryjPark(paidAttractionIds),
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    private static PackageType BuildJaskinie(ProductType lokietka, ProductType ciemna) =>
        new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("OPN — Jaskinie"),
                ProductDescription.Of("Pakiet jaskiniowy: Jaskinia Łokietka i/lub Jaskinia Ciemna. Obie wymagają ciepłego ubrania i stabilnego obuwia."))
            .WithMetadata(SeedHelpers.OPNBase()
                .With("package_type", "jaskinie")
                .With("cold_weather_required", "true"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("jaskinia", 1, 2, lokietka.Id(), ciemna.Id())
                .Build();

    private static PackageType BuildZamkiJurajskie(ProductType zamekOjcow, ProductType zamekPieskowa) =>
        new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("OPN — Zamki Jurajskie"),
                ProductDescription.Of("Bilet łączony na oba zamki OPN: ruiny Kazimierzowskie w Ojcowie i renesansowy Zamek w Pieskowej Skale."))
            .WithMetadata(SeedHelpers.OPNBase()
                .With("package_type", "zamki-jurajskie"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("zamki", 2, 2, zamekOjcow.Id(), zamekPieskowa.Id())
                .Build();

    private static PackageType BuildOdkryjPark(IProductIdentifier[] paidAttractionIds) =>
        new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("OPN — Odkryj Park"),
                ProductDescription.Of("Pakiet zwiedzania indywidualnego OPN: wybór 1–4 płatnych atrakcji (zamki, jaskinie)."))
            .WithMetadata(SeedHelpers.OPNBase()
                .With("package_type", "odkryj-park"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("atrakcje", 1, paidAttractionIds.Length, paidAttractionIds)
                .Build();
}
