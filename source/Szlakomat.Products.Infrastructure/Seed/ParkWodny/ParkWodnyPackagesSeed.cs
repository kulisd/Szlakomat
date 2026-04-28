using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.Seed.ParkWodny;

/// <summary>
/// Seeds package types for Park Wodny Kraków:
/// 1. Bilet Aqua Park (2h / 3h / całodniowy)             — dostęp do basenu, zjeżdżalni, brodzika
/// 2. Bilet Aqua Park + SPA                              — jak wyżej + strefa saun (16+)
/// 3. Bilet Rodzinny (2 dorośli + 2 dzieci)              — pakiet rodzinny całodniowy
/// 4. Bilet Grupowy (min. 10 os.)                        — zniżka grupowa + rezerwacja grupowa
/// 5. Bilet Szkoła / Zorganizowane Grupy Dziecięce       — wycieczka edukacyjna z instruktorem
/// 6. Karnet Miesięczny (Aqua Park)                      — miesięczny dostęp do Aqua Park
/// 7. Karnet Miesięczny (Aqua Park + SPA)                — miesięczny dostęp do Aqua Park + SPA
/// 8. Wizyta Wellness (SPA + masaż 50 min)               — pakiet relaksacyjny dla dorosłych
/// </summary>
internal static class ParkWodnyPackagesSeed
{
    public static List<PackageType> Seed(
        IPackageTypeRepository repo,
        List<ProductType> attractions,
        List<ProductType> services)
    {
        // --- Resolve attractions by name ---
        var basenRek = attractions.First(a => a.Name().Value == "Basen Rekreacyjny");
        var basenSport = attractions.First(a => a.Name().Value == "Basen Sportowy");
        var zjezdzalnie = attractions.First(a => a.Name().Value == "Zjeżdżalnie Wodne");
        var aquaKids = attractions.First(a => a.Name().Value == "Aqua Kids — Strefa dla Dzieci");
        var rzekaJacuzzi = attractions.First(a => a.Name().Value == "Rzeka Leniwca i Jacuzzi");
        var strefaSaun = attractions.First(a => a.Name().Value == "Strefa Saun");
        var basenZew = attractions.First(a => a.Name().Value == "Basen Zewnętrzny");
        var masazKlas = attractions.First(a => a.Name().Value == "Masaż Klasyczny (50 min)");
        var aquaAerobik = attractions.First(a => a.Name().Value == "Aqua Aerobik — zajęcia grupowe");
        var naukaPlyDzieci = attractions.First(a => a.Name().Value == "Nauka Pływania — Dzieci (4–12 lat)");
        var wyporzSprzetu = attractions.First(a => a.Name().Value == "Wypożyczalnia Sprzętu Wodnego");
        var szatnia = attractions.First(a => a.Name().Value == "Szatnia i Przechowalnia bagażu");

        // --- Resolve services by name ---
        var recznik = services.First(s => s.Name().Value == "Wypożyczenie ręcznika");
        var parkomat = services.First(s => s.Name().Value == "Parking — bilet jednorazowy");
        var opaska = services.First(s => s.Name().Value == "Elektroniczna opaska wodoszczelna");

        // All core Aqua Park attractions (no SPA, no outdoor seasonal)
        IProductIdentifier[] coreAquaIds =
        [
            basenRek.Id(), basenSport.Id(), zjezdzalnie.Id(),
            aquaKids.Id(), rzekaJacuzzi.Id(), wyporzSprzetu.Id(), szatnia.Id()
        ];

        // Full set including SPA (16+)
        IProductIdentifier[] aquaAndSpaIds =
        [
            .. coreAquaIds,
            strefaSaun.Id()
        ];

        var packages = new List<PackageType>
        {
            BuildAquaParkTicket(coreAquaIds, recznik, opaska),
            BuildAquaPlusSpaTicket(aquaAndSpaIds, recznik, opaska),
            BuildFamilyTicket(coreAquaIds, recznik, opaska),
            BuildGroupTicket(coreAquaIds, recznik, opaska),
            BuildSchoolGroupTicket(coreAquaIds, naukaPlyDzieci, opaska),
            BuildMonthlyPassAqua(coreAquaIds, opaska),
            BuildMonthlyPassAquaSpa(aquaAndSpaIds, opaska),
            BuildWellnessVisit(strefaSaun, masazKlas, recznik, opaska),
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    // ── 1. Bilet Aqua Park ────────────────────────────────────────────────
    private static PackageType BuildAquaParkTicket(
        IProductIdentifier[] coreAquaIds,
        ProductType recznik,
        ProductType opaska)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet Aqua Park"),
                ProductDescription.Of("Bilet wstępu do strefy Aqua Park (baseny, zjeżdżalnie, brodzik, rzeka, jacuzzi). Szafka i opaska w cenie. Ręcznik opcjonalnie."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "aqua_park_ticket")
                .With("price_2h_normalny_pln", "35")
                .With("price_2h_ulgowy_pln", "27")
                .With("price_3h_normalny_pln", "43")
                .With("price_3h_ulgowy_pln", "33")
                .With("price_full_normalny_pln", "55")
                .With("price_full_ulgowy_pln", "42")
                .With("price_dzieciecy_pln", "28")
                .With("locker_and_wristband_included", "true"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("aqua_park_attractions", 1, coreAquaIds.Length, coreAquaIds)
                .WithSingleChoice("wristband", opaska.Id())
                .WithOptionalChoice("towel_rental", recznik.Id())
                .Build();
    }

    // ── 2. Bilet Aqua Park + SPA ──────────────────────────────────────────
    private static PackageType BuildAquaPlusSpaTicket(
        IProductIdentifier[] aquaAndSpaIds,
        ProductType recznik,
        ProductType opaska)
    {
        // SPA wymaga 16 lat
        var adultConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 15);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet Aqua Park + SPA"),
                ProductDescription.Of("Bilet łączony: pełny dostęp do Aqua Park i strefy saun. Tylko dla gości 16+. Szafka i opaska w cenie."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "aqua_spa_ticket")
                .With("min_age", "16")
                .With("price_normalny_pln", "75")
                .With("price_ulgowy_pln", "58")
                .With("locker_and_wristband_included", "true"))
            .WithApplicabilityConstraint(adultConstraint)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("aqua_spa_attractions", 1, aquaAndSpaIds.Length, aquaAndSpaIds)
                .WithSingleChoice("wristband", opaska.Id())
                .WithOptionalChoice("towel_rental", recznik.Id())
                .Build();
    }

    // ── 3. Bilet Rodzinny ────────────────────────────────────────────────
    private static PackageType BuildFamilyTicket(
        IProductIdentifier[] coreAquaIds,
        ProductType recznik,
        ProductType opaska)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet Rodzinny (2+2)"),
                ProductDescription.Of("Bilet rodzinny całodniowy dla 2 dorosłych i 2 dzieci do 15 lat. Szafki i opaski w cenie. Oszczędność do 25% w stosunku do biletów indywidualnych."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "family_ticket")
                .With("adults", "2")
                .With("children_max_age", "15")
                .With("children", "2")
                .With("price_pln", "129")
                .With("discount_vs_individual_pct", "25")
                .With("locker_and_wristband_included", "true"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("aqua_park_attractions", 1, coreAquaIds.Length, coreAquaIds)
                .WithSingleChoice("wristband", opaska.Id())
                .WithOptionalChoice("towel_rental", recznik.Id())
                .Build();
    }

    // ── 4. Bilet Grupowy (min. 10 osób) ──────────────────────────────────
    private static PackageType BuildGroupTicket(
        IProductIdentifier[] coreAquaIds,
        ProductType recznik,
        ProductType opaska)
    {
        var groupConstraint = ApplicabilityConstraint.And(
            ApplicabilityConstraint.GreaterThan("group_size", 9),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 2));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet Grupowy (min. 10 osób)"),
                ProductDescription.Of("Bilet grupowy całodniowy dla zorganizowanych grup minimum 10-osobowych. Wymagana rezerwacja min. 3 dni wcześniej. Zniżka grupowa 20%."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "group_ticket")
                .With("min_group_size", "10")
                .With("price_normalny_pln", "44")
                .With("price_ulgowy_pln", "34")
                .With("advance_booking_days", "3")
                .With("discount_vs_individual_pct", "20"))
            .WithApplicabilityConstraint(groupConstraint)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("aqua_park_attractions", 1, coreAquaIds.Length, coreAquaIds)
                .WithSingleChoice("wristband", opaska.Id())
                .WithOptionalChoice("towel_rental", recznik.Id())
                .Build();
    }

    // ── 5. Bilet Szkoła / Grupa Zorganizowana ────────────────────────────
    private static PackageType BuildSchoolGroupTicket(
        IProductIdentifier[] coreAquaIds,
        ProductType naukaPlyDzieci,
        ProductType opaska)
    {
        var schoolConstraint = ApplicabilityConstraint.And(
            ApplicabilityConstraint.GreaterThan("group_size", 9),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 6));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet Szkoła — Wycieczka Edukacyjna"),
                ProductDescription.Of("Pakiet dla zorganizowanych grup szkolnych: 2h Aqua Park + opcjonalna lekcja pływania z instruktorem PZP. Rezerwacja min. 7 dni wcześniej."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "school_group")
                .With("min_group_size", "10")
                .With("duration_h", "2")
                .With("price_uczen_pln", "28")
                .With("advance_booking_days", "7")
                .With("teacher_free", "true")
                .With("teacher_to_students_ratio", "1:15"))
            .WithApplicabilityConstraint(schoolConstraint)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("aqua_park_attractions", 1, coreAquaIds.Length, coreAquaIds)
                .WithOptionalChoice("swimming_lesson", naukaPlyDzieci.Id())
                .WithSingleChoice("wristband", opaska.Id())
                .Build();
    }

    // ── 6. Karnet Miesięczny — Aqua Park ─────────────────────────────────
    private static PackageType BuildMonthlyPassAqua(
        IProductIdentifier[] coreAquaIds,
        ProductType opaska)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Karnet Miesięczny — Aqua Park"),
                ProductDescription.Of("30-dniowy karnet na nielimitowane wejścia do strefy Aqua Park. Imienny, nieprzenoszalny."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "monthly_pass_aqua")
                .With("price_normalny_pln", "199")
                .With("price_ulgowy_pln", "149")
                .With("validity_days", "30")
                .With("personal", "true")
                .With("transferable", "false"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("aqua_park_attractions", 1, coreAquaIds.Length, coreAquaIds)
                .WithSingleChoice("wristband", opaska.Id())
                .Build();
    }

    // ── 7. Karnet Miesięczny — Aqua Park + SPA ───────────────────────────
    private static PackageType BuildMonthlyPassAquaSpa(
        IProductIdentifier[] aquaAndSpaIds,
        ProductType opaska)
    {
        var adultConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 15);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Karnet Miesięczny — Aqua Park + SPA"),
                ProductDescription.Of("30-dniowy karnet na nielimitowane wejścia do strefy Aqua Park i SPA (sauna). Tylko dla gości 16+. Imienny, nieprzenoszalny."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "monthly_pass_aqua_spa")
                .With("min_age", "16")
                .With("price_normalny_pln", "299")
                .With("price_ulgowy_pln", "229")
                .With("validity_days", "30")
                .With("personal", "true")
                .With("transferable", "false"))
            .WithApplicabilityConstraint(adultConstraint)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithChoice("aqua_spa_attractions", 1, aquaAndSpaIds.Length, aquaAndSpaIds)
                .WithSingleChoice("wristband", opaska.Id())
                .Build();
    }

    // ── 8. Wizyta Wellness ────────────────────────────────────────────────
    private static PackageType BuildWellnessVisit(
        ProductType strefaSaun,
        ProductType masazKlas,
        ProductType recznik,
        ProductType opaska)
    {
        var adultConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 15);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Wizyta Wellness — SPA + Masaż"),
                ProductDescription.Of("Pakiet wellness: 3h dostęp do strefy saun + masaż klasyczny 50 min + ręcznik. Wyłącznie dla gości 16+. Rezerwacja min. 24h wcześniej."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("package_type", "wellness_visit")
                .With("min_age", "16")
                .With("price_normalny_pln", "185")
                .With("advance_booking_h", "24")
                .With("spa_duration_h", "3")
                .With("massage_duration_min", "50"))
            .WithApplicabilityConstraint(adultConstraint)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithSingleChoice("sauna_zone", strefaSaun.Id())
                .WithSingleChoice("massage", masazKlas.Id())
                .WithSingleChoice("towel_rental", recznik.Id())
                .WithSingleChoice("wristband", opaska.Id())
                .Build();
    }
}
