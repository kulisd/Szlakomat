using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Infrastructure.Seed;

/// <summary>
/// Shared metadata builder helpers for Wawel seed data.
/// </summary>
internal static class SeedHelpers
{
    /// <summary>
    /// Returns base metadata common to all Wawel products.
    /// </summary>
    public static ProductMetadata WawelBase() =>
        ProductMetadata.Empty()
            .With("city", "Kraków")
            .With("district", "Wawel")
            .With("category", "zamek-muzeum")
            .With("unesco", "true")
            .With("accessibility", "partial");

    /// <summary>
    /// Returns base metadata for Wawel exhibitions (indoor, timed entry).
    /// </summary>
    public static ProductMetadata WawelExhibitionBase() =>
        WawelBase()
            .With("indoor_outdoor", "indoor")
            .With("requires_timed_entry", "true");

    /// <summary>
    /// Returns base metadata common to all Bazylika Mariacka products.
    /// </summary>
    public static ProductMetadata MariackaBase() =>
        ProductMetadata.Empty()
            .With("city", "Kraków")
            .With("district", "Śródmieście")
            .With("address", "Plac Mariacki 5, 31-042 Kraków")
            .With("category", "kościół-bazylika")
            .With("unesco", "false")
            .With("accessibility", "limited");

    /// <summary>
    /// Returns base metadata for Bazylika Mariacka exhibits/routes (indoor).
    /// </summary>
    public static ProductMetadata MariackaRouteBase() =>
        MariackaBase()
            .With("indoor_outdoor", "indoor")
            .With("attraction_type", "religious-heritage");

    /// <summary>
    /// Returns contact/booking metadata for Bazylika Mariacka.
    /// </summary>
    public static ProductMetadata MariackaContactInfo() =>
        ProductMetadata.Empty()
            .With("website", "www.mariacki.com")
            .With("phone", "+48 12 422 05 21")
            .With("group_booking_note", "Grupy zorganizowane: wymagana wcześniejsza rezerwacja telefoniczna lub mailowa")
            .With("group_booking_email", "muzeum@mariacki.com")
            .With("max_group_size", "30");

    /// <summary>
    /// Returns metadata for reservation office details.
    /// </summary>
    public static ProductMetadata WawelReservationInfo() =>
        ProductMetadata.Empty()
            .With("reservation_office_hours_mon", "09:00-14:00")
            .With("reservation_office_hours_tue_sun", "09:00-16:00")
            .With("reservation_office_closed", "1 I, Wielkanoc, 1 XI, 11 XI, 24 XII, 25 XII")
            .With("reservation_email", "rezerwacja@wawelzamek.pl")
            .With("reservation_phone", "+48 12 422 16 97")
            .With("max_group_size", "30")
            .With("groups_above_30_split", "true")
            .With("confirmation_deadline_days_before", "3");
}