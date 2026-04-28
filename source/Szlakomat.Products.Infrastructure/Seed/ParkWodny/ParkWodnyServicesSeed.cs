using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.ParkWodny;

/// <summary>
/// Seeds Park Wodny Kraków services (ręcznik, szafka, instruktor,
/// parking, zegarek waterproof) as ProductType instances.
/// </summary>
internal static class ParkWodnyServicesSeed
{
    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var services = new List<ProductType>
        {
            BuildTowelRental(),
            BuildLockerRental(),
            BuildParkingTicket(),
            BuildSwimmingInstructor(),
            BuildWaterproofWristband(),
        };

        foreach (var service in services)
        {
            repo.Save(service);
        }

        return services;
    }

    private static ProductType BuildTowelRental()
    {
        return new ProductBuilder(
                UuidProductIdentifier.Of("b2c3d4e5-0001-4000-8000-000000000001"),
                ProductName.Of("Wypożyczenie ręcznika"),
                ProductDescription.Of("Wypożyczenie dużego ręcznika kąpielowego na czas wizyty w parku wodnym. Zwrot przy wyjściu."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("service_type", "towel_rental")
                .With("price_pln", "10")
                .With("deposit_pln", "20"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();
    }

    private static ProductType BuildLockerRental()
    {
        return new ProductBuilder(
                UuidProductIdentifier.Of("b2c3d4e5-0002-4000-8000-000000000002"),
                ProductName.Of("Dodatkowa szafka bagażowa"),
                ProductDescription.Of("Dodatkowa szafka do przechowywania mienia (standardowo 1 szafka wliczona w bilet). Dostępne szafki małe i duże."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("service_type", "extra_locker")
                .With("price_small_pln", "5")
                .With("price_large_pln", "10")
                .With("deposit_pln", "20"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("locker_size", "mała", "duża"))
                .Build();
    }

    private static ProductType BuildParkingTicket()
    {
        return new ProductBuilder(
                UuidProductIdentifier.Of("b2c3d4e5-0003-4000-8000-000000000003"),
                ProductName.Of("Parking — bilet jednorazowy"),
                ProductDescription.Of("Bilet na parking przy ul. Dobrego Pasterza 126. Parking płatny, pojemność 200 miejsc."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("service_type", "parking")
                .With("price_2h_pln", "6")
                .With("price_4h_pln", "10")
                .With("price_full_day_pln", "15")
                .With("capacity_spots", "200")
                .With("address", "ul. Dobrego Pasterza 126"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("parking_duration", "2h", "4h", "całodziennie"))
                .Build();
    }

    private static ProductType BuildSwimmingInstructor()
    {
        // Instruktor indywidualny — rezerwacja minimum 48h z góry
        var bookingConstraint = ApplicabilityConstraint.GreaterThan("booking_hours_ahead", 47);

        return new ProductBuilder(
                UuidProductIdentifier.Of("b2c3d4e5-0004-4000-8000-000000000004"),
                ProductName.Of("Instruktor pływania — lekcja indywidualna"),
                ProductDescription.Of("Indywidualna lekcja pływania z certyfikowanym instruktorem (licencja PZP). Dostępne dla dzieci (4–15 lat) i dorosłych. Rezerwacja min. 48h wcześniej."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("service_type", "swimming_instructor")
                .With("lesson_duration_min", "45")
                .With("price_child_pln", "80")
                .With("price_adult_pln", "100")
                .With("advance_booking_h", "48")
                .With("instructor_license", "PZP")
                .With("max_concurrent_students", "1"))
            .WithApplicabilityConstraint(bookingConstraint)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("student_type", "dziecko", "dorosły"))
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("student_age", 4, 99))
                .Build();
    }

    private static ProductType BuildWaterproofWristband()
    {
        return new ProductBuilder(
                UuidProductIdentifier.Of("b2c3d4e5-0005-4000-8000-000000000005"),
                ProductName.Of("Elektroniczna opaska wodoszczelna"),
                ProductDescription.Of("Elektroniczna opaska wstępna zastępująca klucz do szafki i służąca jako portmonetka bezgotówkowa na terenie parku."))
            .WithMetadata(SeedHelpers.ParkWodnyBase()
                .With("service_type", "wristband")
                .With("included_in_ticket", "true")
                .With("cashless_payments", "true")
                .With("deposit_pln", "20"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();
    }
}
