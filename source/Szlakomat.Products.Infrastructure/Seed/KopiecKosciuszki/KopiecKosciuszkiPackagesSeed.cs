using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.Seed.KopiecKosciuszki;

/// <summary>
/// Seeds Kopiec Kościuszki package types:
/// 1. Bilet wstępu na Kopiec                — wejście na kopiec + trasa fortyfikacyjna
/// 2. Bilet łączony: Kopiec + Muzeum         — kopiec + ekspozycja stała muzeum
/// 3. Bilet łączony: Kopiec + Multimedia     — kopiec + wystawa multimedialna
/// 4. Bilet kompleksowy                      — kopiec + muzeum + multimedia + galeria
/// 5. Bilet rodzinny                         — 2 dorosłych + max 3 dzieci (bilet łączony)
/// 6. Wizyta grupowa z przewodnikiem         — bilet łączony + przewodnik + opcja audioprzewodnika
/// </summary>
internal static class KopiecKosciuszkiPackagesSeed
{
    public static List<PackageType> Seed(
        IPackageTypeRepository repo,
        List<ProductType> attractions,
        List<ProductType> services)
    {
        var guidePl = services.First(s => s.Name().Value.Contains("polski"));
        var guideForeign = services.First(s => s.Name().Value.Contains("obcy"));
        var audioGuide = services.First(s => s.Name().Value.Contains("Audioprzewodnik"));
        var locker = services.First(s => s.Name().Value.Contains("Szafka"));

        var kopiec = attractions.First(a => a.Name().Value == "Wejście na Kopiec Kościuszki");
        var muzeum = attractions.First(a => a.Name().Value == "Muzeum im. Tadeusza Kościuszki");
        var galeria = attractions.First(a => a.Name().Value == "Galeria Sztuki Patriotycznej");
        var multimedia = attractions.First(a => a.Name().Value.StartsWith("Wystawa multimedialna"));
        var fortTrail = attractions.First(a => a.Name().Value.StartsWith("Trasa Fortyfikacyjna"));

        var packages = new List<PackageType>
        {
            BuildKopiecOnly(kopiec, fortTrail),
            BuildKopiecPlusMuzeum(kopiec, muzeum, fortTrail, audioGuide, locker),
            BuildKopiecPlusMultimedia(kopiec, multimedia, fortTrail, audioGuide),
            BuildKopiecKompleksowy(kopiec, muzeum, galeria, multimedia, fortTrail, audioGuide, locker),
            BuildFamilyTicket(kopiec, muzeum, fortTrail),
            BuildGroupVisit(kopiec, muzeum, multimedia, fortTrail, guidePl, guideForeign, audioGuide, locker),
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    /// <summary>
    /// Bilet wstępu na Kopiec — wejście na kopiec + trasa fortyfikacyjna (gratis).
    /// Cena: 16 PLN normalny / 10 PLN ulgowy.
    /// </summary>
    private static PackageType BuildKopiecOnly(ProductType kopiec, ProductType fortTrail)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet wstępu na Kopiec Kościuszki"),
                ProductDescription.Of("Wejście na Kopiec Kościuszki z panoramą 360° Krakowa. Obejmuje trasę fortyfikacyjną (gratis)."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("package_type", "kopiec_only")
                .With("price_normalny_pln", "16")
                .With("price_ulgowy_pln", "10")
                .With("featured", "true")
                .With("badge", "polecany"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithSingleChoice("kopiec", kopiec.Id())
                .WithSingleChoice("fort_trail", fortTrail.Id())
                .Build();
    }

    /// <summary>
    /// Bilet łączony: Kopiec + Muzeum.
    /// Cena: 25 PLN normalny / 16 PLN ulgowy. Audioprzewodnik opcjonalny.
    /// </summary>
    private static PackageType BuildKopiecPlusMuzeum(
        ProductType kopiec,
        ProductType muzeum,
        ProductType fortTrail,
        ProductType audioGuide,
        ProductType locker)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet łączony: Kopiec + Muzeum Kościuszki"),
                ProductDescription.Of("Bilet łączony na wejście na kopiec i ekspozycję stałą Muzeum im. Tadeusza Kościuszki."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("package_type", "kopiec_muzeum_combined")
                .With("price_normalny_pln", "25")
                .With("price_ulgowy_pln", "16")
                .With("featured", "true")
                .With("badge", "bestseller"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithSingleChoice("kopiec", kopiec.Id())
                .WithSingleChoice("muzeum", muzeum.Id())
                .WithSingleChoice("fort_trail", fortTrail.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .WithOptionalChoice("luggage_locker", locker.Id())
                .Build();
    }

    /// <summary>
    /// Bilet łączony: Kopiec + Wystawa multimedialna.
    /// Cena: 28 PLN normalny / 18 PLN ulgowy.
    /// </summary>
    private static PackageType BuildKopiecPlusMultimedia(
        ProductType kopiec,
        ProductType multimedia,
        ProductType fortTrail,
        ProductType audioGuide)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet łączony: Kopiec + Wystawa Multimedialna"),
                ProductDescription.Of("Bilet łączony na wejście na kopiec i nowoczesną wystawę multimedialną „Kościuszko — Człowiek i Symbol"."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("package_type", "kopiec_multimedia_combined")
                .With("price_normalny_pln", "28")
                .With("price_ulgowy_pln", "18")
                .With("badge", "nowość"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithSingleChoice("kopiec", kopiec.Id())
                .WithSingleChoice("multimedia", multimedia.Id())
                .WithSingleChoice("fort_trail", fortTrail.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .Build();
    }

    /// <summary>
    /// Bilet kompleksowy — kopiec + muzeum + multimedia + galeria + trasa.
    /// Cena: 45 PLN normalny / 30 PLN ulgowy. Audioprzewodnik w cenie.
    /// </summary>
    private static PackageType BuildKopiecKompleksowy(
        ProductType kopiec,
        ProductType muzeum,
        ProductType galeria,
        ProductType multimedia,
        ProductType fortTrail,
        ProductType audioGuide,
        ProductType locker)
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet kompleksowy — Kopiec i Fort"),
                ProductDescription.Of("Pełen bilet łączony: kopiec, muzeum, wystawa multimedialna, galeria sztuki, trasa fortyfikacyjna. Audioprzewodnik w cenie."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("package_type", "kopiec_kompleksowy")
                .With("price_normalny_pln", "45")
                .With("price_ulgowy_pln", "30")
                .With("audio_in_price", "true")
                .With("badge", "must-see"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.IndividuallyTracked)
                .WithSingleChoice("kopiec", kopiec.Id())
                .WithSingleChoice("muzeum", muzeum.Id())
                .WithSingleChoice("galeria", galeria.Id())
                .WithSingleChoice("multimedia", multimedia.Id())
                .WithSingleChoice("fort_trail", fortTrail.Id())
                .WithSingleChoice("audio_guide", audioGuide.Id())
                .WithOptionalChoice("luggage_locker", locker.Id())
                .Build();
    }

    /// <summary>
    /// Bilet rodzinny — 2 dorosłych + max 3 dzieci, bilet łączony (kopiec + muzeum).
    /// Cena: 55 PLN za rodzinę.
    /// </summary>
    private static PackageType BuildFamilyTicket(
        ProductType kopiec,
        ProductType muzeum,
        ProductType fortTrail)
    {
        // Bilet rodzinny: dzieci poniżej 16. roku życia
        var familyApplicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("adults_count", 1, 2),
            ApplicabilityConstraint.Between("children_count", 1, 3));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Bilet rodzinny — Kopiec i Muzeum"),
                ProductDescription.Of("Bilet rodzinny na wejście na kopiec i muzeum: 2 dorosłych + max 3 dzieci do lat 16."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("package_type", "family_ticket")
                .With("price_pln", "55")
                .With("max_adults", "2")
                .With("max_children", "3")
                .With("children_max_age", "15")
                .With("badge", "rodzinny"))
            .WithApplicabilityConstraint(familyApplicability)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithSingleChoice("kopiec", kopiec.Id())
                .WithSingleChoice("muzeum", muzeum.Id())
                .WithSingleChoice("fort_trail", fortTrail.Id())
                .Build();
    }

    /// <summary>
    /// Wizyta grupowa z przewodnikiem — kopiec + wybór atrakcji + przewodnik PL/obcy + opcje.
    /// Grupy 5–25 osób, rezerwacja min. 7 dni wcześniej.
    /// </summary>
    private static PackageType BuildGroupVisit(
        ProductType kopiec,
        ProductType muzeum,
        ProductType multimedia,
        ProductType fortTrail,
        ProductType guidePl,
        ProductType guideForeign,
        ProductType audioGuide,
        ProductType locker)
    {
        var groupApplicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 5, 25),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 6));

        var allAttractionIds = new[] { kopiec.Id(), muzeum.Id(), multimedia.Id(), fortTrail.Id() };

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Wizyta grupowa z przewodnikiem — Kopiec Kościuszki"),
                ProductDescription.Of("Pakiet grupowy: wybór atrakcji, licencjonowany przewodnik (PL lub obcojęzyczny), opcjonalny audioprzewodnik i szafki. Grupy 5–25 osób."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("package_type", "group_guided_visit")
                .With("min_group", "5")
                .With("max_group", "25"))
            .WithApplicabilityConstraint(groupApplicability)
            .AsPackageType()
                .WithTrackingStrategy(ProductTrackingStrategy.BatchTracked)
                .WithChoice("attractions", 1, allAttractionIds.Length, allAttractionIds)
                .WithSingleChoice("guide", guidePl.Id(), guideForeign.Id())
                .WithOptionalChoice("audio_guide", audioGuide.Id())
                .WithOptionalChoice("luggage_locker", locker.Id())
                .Build();
    }
}
