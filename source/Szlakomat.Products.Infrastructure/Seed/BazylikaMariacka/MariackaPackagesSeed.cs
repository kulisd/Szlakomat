using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.Seed.BazylikaMariacka;

/// <summary>
/// Seeds package types for Bazylika Mariacka.
/// </summary>
internal static class MariackaPackagesSeed
{
    public static List<PackageType> Seed(
        IPackageTypeRepository repo,
        ProductType basilikaInterior,
        ProductType hejnalica,
        List<ProductType> services)
    {
        var guidePl = RequireService(services, "polski");
        var guideForeign = RequireService(services, "obcy");
        var audioGuide = RequireService(services, "Audioprzewodnik");
        var groupHeadsets = RequireService(services, "Słuchawki");

        var packages = new List<PackageType>
        {
            BuildIndividualVisit(basilikaInterior, hejnalica, audioGuide),
            BuildGuidedGroupTour(basilikaInterior, guidePl, guideForeign, groupHeadsets),
            BuildCombinedTicket(basilikaInterior, hejnalica, audioGuide),
            BuildSchoolVisit(basilikaInterior, audioGuide, guidePl, groupHeadsets),
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    // -----------------------------------------------------------------------
    // SAFE SERVICE RESOLUTION
    // -----------------------------------------------------------------------
    private static ProductType RequireService(List<ProductType> services, string keyword)
    {
        var match = services.FirstOrDefault(s =>
            s.Name().Value.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            var available = string.Join(", ", services.Select(s => s.Name().Value));

            throw new InvalidOperationException(
                $"MariackaPackagesSeed: Missing service containing '{keyword}'. " +
                $"Available services: {available}");
        }

        return match;
    }

    // -----------------------------------------------------------------------
    // 1. Individual visit
    // -----------------------------------------------------------------------
    private static PackageType BuildIndividualVisit(
        ProductType basilikaInterior,
        ProductType hejnalica,
        ProductType audioGuide)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bazylika Mariacka — Wizyta indywidualna"),
                ProductDescription.Of(
                    "Pakiet zwiedzania indywidualnego: wybierz jedną lub obie trasy " +
                    "(wnętrze Bazyliki i/lub Hejnalica) z opcjonalnym audioprzewodnikiem."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("package_type", "individual_visit"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("routes", 1, 2, basilikaInterior.Id(), hejnalica.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .Build();
    }

    // -----------------------------------------------------------------------
    // 2. Guided group tour
    // -----------------------------------------------------------------------
    private static PackageType BuildGuidedGroupTour(
        ProductType basilikaInterior,
        ProductType guidePl,
        ProductType guideForeign,
        ProductType groupHeadsets)
    {
        var applicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 1));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bazylika Mariacka — Wycieczka grupowa z przewodnikiem"),
                ProductDescription.Of(
                    "Grupowe zwiedzanie z przewodnikiem (PL lub EN/DE/FR)."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("package_type", "guided_group_tour"))
            .WithApplicabilityConstraint(applicability)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithSingleChoice("route", basilikaInterior.Id())
                .WithSingleChoice("guide", guidePl.Id(), guideForeign.Id())
                .WithOptionalChoice("group_headsets", groupHeadsets.Id())
                .Build();
    }

    // -----------------------------------------------------------------------
    // 3. Combined ticket
    // -----------------------------------------------------------------------
    private static PackageType BuildCombinedTicket(
        ProductType basilikaInterior,
        ProductType hejnalica,
        ProductType audioGuide)
    {
        var ageConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 6);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bazylika Mariacka — Bilet łączony"),
                ProductDescription.Of(
                    "Wnętrze + Hejnalica w jednym bilecie."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("package_type", "combined_ticket"))
            .WithApplicabilityConstraint(ageConstraint)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("routes", 2, 2, basilikaInterior.Id(), hejnalica.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .Build();
    }

    // -----------------------------------------------------------------------
    // 4. School visit
    // -----------------------------------------------------------------------
    private static PackageType BuildSchoolVisit(
        ProductType basilikaInterior,
        ProductType audioGuide,
        ProductType guidePl,
        ProductType groupHeadsets)
    {
        var applicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 10, 30),
            ApplicabilityConstraint.Between("visitor_age", 7, 18),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 2));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bazylika Mariacka — Szkolna wizyta"),
                ProductDescription.Of(
                    "Pakiet edukacyjny dla szkół."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("package_type", "school_educational"))
            .WithApplicabilityConstraint(applicability)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithSingleChoice("interior", basilikaInterior.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .WithOptionalChoice("guide_pl", guidePl.Id())
                .WithOptionalChoice("group_headsets", groupHeadsets.Id())
                .Build();
    }
}