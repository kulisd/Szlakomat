using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.Seed.Wawel;

/// <summary>
/// Seeds Wawel package types:
/// 1. Individual Visit — choose exhibitions + optional audio guide + mandatory luggage storage
/// 2. Guided Group Tour — exhibitions + guide + optional headsets + luggage + optional audio
/// 3. Zamek (piętro 1+2) — combined castle ticket with audioguide included
/// 4. Bilet roczny — annual pass (260/195 PLN)
/// 5. Wawel dla pasjonatów — combined ticket (199/149 PLN)
/// 6. Najważniejsze na Wawelu — family trail with educator (39 PLN/os)
/// 7. Wawel – najcenniejsze — guided premium trail (155/116 PLN)
/// 8. Szukając Smoka — seasonal combined trail Międzymurze + Smocza Jama (35/26 PLN)
/// </summary>
internal static class WawelPackagesSeed
{
    public static List<PackageType> Seed(
        IPackageTypeRepository repo,
        List<ProductType> exhibitions,
        List<ProductType> services)
    {
        // Extract service IDs by name prefix for clarity
        var guidePl = services.First(s => s.Name().Value.Contains("polski"));
        var guideForeign = services.First(s => s.Name().Value.Contains("obcy"));
        var audioGuide = services.First(s => s.Name().Value.Contains("Audioprzewodnik"));
        var luggage = services.First(s => s.Name().Value.Contains("Przechowalnia"));
        var headsets = services.First(s => s.Name().Value.Contains("słuchawek"));

        var allExhibitionIds = exhibitions.Select(e => e.Id()).ToArray();

        // Extract specific exhibitions for combined packages
        var pietro1 = exhibitions.First(e => e.Name().Value == "Prywatne Apartamenty Królewskie");
        var pietro2 = exhibitions.First(e => e.Name().Value == "Reprezentacyjne Komnaty Królewskie");
        var skarbiec = exhibitions.First(e => e.Name().Value == "Skarbiec Koronny");
        var zbrojownia = exhibitions.First(e => e.Name().Value == "Zbrojownia");
        var podziemia = exhibitions.First(e => e.Name().Value == "Podziemia Zamku");
        var miedzymurze = exhibitions.First(e => e.Name().Value.StartsWith("Międzymurze"));
        var wawelOdzyskany = exhibitions.First(e => e.Name().Value == "Wawel Odzyskany");
        var smoczaJama = exhibitions.First(e => e.Name().Value == "Smocza Jama");

        var packages = new List<PackageType>
        {
            BuildIndividualVisit(allExhibitionIds, audioGuide, luggage),
            BuildGuidedGroupTour(allExhibitionIds, guidePl, guideForeign, audioGuide, luggage, headsets),
            BuildCastleCombined(pietro1, pietro2, wawelOdzyskany, audioGuide, luggage),
            BuildAnnualPass(allExhibitionIds),
            BuildWawelDlaPasjonatow(allExhibitionIds, audioGuide, luggage),
            BuildFamilyTrail(allExhibitionIds, luggage),
            BuildPremiumGuidedTrail(allExhibitionIds, guidePl, guideForeign, luggage),
            BuildSzukajacSmoka(miedzymurze, smoczaJama),
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    private static PackageType BuildIndividualVisit(
        IProductIdentifier[] exhibitionIds,
        ProductType audioGuide,
        ProductType luggage)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Zamek Królewski na Wawelu — Wizyta indywidualna"),
                ProductDescription.Of("Pakiet zwiedzania indywidualnego: wybór 1–5 wystaw, opcjonalny audioprzewodnik, obowiązkowa przechowalnia bagażu."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "individual_visit"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("exhibitions", 1, 5, exhibitionIds)
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .WithSingleChoice("luggage_storage", luggage.Id())
                .Build();
    }

    private static PackageType BuildGuidedGroupTour(
        IProductIdentifier[] exhibitionIds,
        ProductType guidePl,
        ProductType guideForeign,
        ProductType audioGuide,
        ProductType luggage,
        ProductType headsets)
    {
        // Group 1-30 + 14 days advance booking
        var applicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 13));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Zamek Królewski na Wawelu — Wycieczka grupowa z przewodnikiem"),
                ProductDescription.Of("Pakiet grupowy: wybór 1–5 wystaw, przewodnik (PL lub obcojęzyczny), opcjonalne słuchawki i audioprzewodnik, obowiązkowa przechowalnia."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "guided_group_tour"))
            .WithApplicabilityConstraint(applicability)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("exhibitions", 1, 5, exhibitionIds)
                .WithSingleChoice("guide", guidePl.Id(), guideForeign.Id())
                .WithOptionalChoice("headsets", headsets.Id())
                .WithSingleChoice("luggage_storage", luggage.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .Build();
    }

    private static PackageType BuildCastleCombined(
        ProductType pietro1,
        ProductType pietro2,
        ProductType wawelOdzyskany,
        ProductType audioGuide,
        ProductType luggage)
    {
        // Zamek (piętro 1+2) — combined ticket: 95/71 PLN, audioguide included
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Zamek (piętro 1+2)"),
                ProductDescription.Of("Bilet łączony na piętro 1 i 2 Zamku Królewskiego z audioprzewodnikiem w cenie. Obejmuje także Wawel Odzyskany."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "castle_combined")
                .With("price_standard_pln", "95")
                .With("price_reduced_pln", "71")
                .With("audio_in_price", "true")
                .With("last_entry_min_before_close", "110"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("castle_floors", 2, 2, pietro1.Id(), pietro2.Id())
                .WithSingleChoice("wawel_odzyskany", wawelOdzyskany.Id())
                .WithSingleChoice("audio_guide", audioGuide.Id())
                .WithSingleChoice("luggage_storage", luggage.Id())
                .Build();
    }

    private static PackageType BuildAnnualPass(IProductIdentifier[] exhibitionIds)
    {
        // Bilet roczny: 260/195 PLN
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet roczny"),
                ProductDescription.Of("Roczny bilet wstępu na wszystkie wystawy stałe Zamku Królewskiego na Wawelu."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "annual_pass")
                .With("price_standard_pln", "260")
                .With("price_reduced_pln", "195")
                .With("validity_months", "12"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("exhibitions", 1, exhibitionIds.Length, exhibitionIds)
                .Build();
    }

    private static PackageType BuildWawelDlaPasjonatow(
        IProductIdentifier[] exhibitionIds,
        ProductType audioGuide,
        ProductType luggage)
    {
        // Wawel dla pasjonatów: 199/149 PLN — combined ticket for multiple exhibitions
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Wawel dla pasjonatów"),
                ProductDescription.Of("Bilet łączony na wiele wystaw Zamku Królewskiego — dla osób chcących zobaczyć jak najwięcej."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "enthusiast_combined")
                .With("price_standard_pln", "199")
                .With("price_reduced_pln", "149"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("exhibitions", 3, exhibitionIds.Length, exhibitionIds)
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .WithSingleChoice("luggage_storage", luggage.Id())
                .Build();
    }

    private static PackageType BuildFamilyTrail(
        IProductIdentifier[] exhibitionIds,
        ProductType luggage)
    {
        // Najważniejsze na Wawelu — trasa rodzinna z edukatorem: 39 PLN/os
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Najważniejsze na Wawelu — trasa rodzinna z edukatorem"),
                ProductDescription.Of("Trasa rodzinna z edukatorem po najważniejszych punktach Wawelu. Cena: 39 PLN za osobę."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "family_trail")
                .With("price_per_person_pln", "39")
                .With("with_educator", "true"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("exhibitions", 1, exhibitionIds.Length, exhibitionIds)
                .WithSingleChoice("luggage_storage", luggage.Id())
                .Build();
    }

    private static PackageType BuildSzukajacSmoka(
        ProductType miedzymurze,
        ProductType smoczaJama)
    {
        // Szukając Smoka — sezonowa trasa łączona: Międzymurze + Smocza Jama
        // Sezon: 24 IV - 31 X. Cena: 35/26 PLN.
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Szukając Smoka"),
                ProductDescription.Of("Sezonowa trasa łączona po Międzymurzu i Smoczej Jamie (24 IV – 31 X)."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "szukajac_smoka")
                .With("price_standard_pln", "35")
                .With("price_reduced_pln", "26")
                .With("seasonal", "true")
                .With("season_start", "24 IV")
                .With("season_end", "31 X"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("trail_stops", 2, 2, miedzymurze.Id(), smoczaJama.Id())
                .Build();
    }

    private static PackageType BuildPremiumGuidedTrail(
        IProductIdentifier[] exhibitionIds,
        ProductType guidePl,
        ProductType guideForeign,
        ProductType luggage)
    {
        // Wawel – najcenniejsze — trasa z przewodnikiem: 155/116 PLN
        var applicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 13));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Wawel – najcenniejsze — trasa z przewodnikiem"),
                ProductDescription.Of("Trasa z licencjonowanym przewodnikiem po najcenniejszych zbiorach Wawelu."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("package_type", "premium_guided_trail")
                .With("price_standard_pln", "155")
                .With("price_reduced_pln", "116"))
            .WithApplicabilityConstraint(applicability)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("exhibitions", 2, exhibitionIds.Length, exhibitionIds)
                .WithSingleChoice("guide", guidePl.Id(), guideForeign.Id())
                .WithSingleChoice("luggage_storage", luggage.Id())
                .Build();
    }
}
