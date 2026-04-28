using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Infrastructure.Seed;

/// <summary>
/// Shared metadata builder helpers for seed data (Wawel, Park Wodny, ...).
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

    // ── Park Wodny Kraków helpers ────────────────────────────────────────

    /// <summary>
    /// Returns base metadata common to all Park Wodny Kraków products.
    /// Adres: ul. Dobrego Pasterza 126, 31-416 Kraków.
    /// </summary>
    public static ProductMetadata ParkWodnyBase() =>
        ProductMetadata.Empty()
            .With("city", "Kraków")
            .With("district", "Prądnik Czerwony")
            .With("category", "park-wodny")
            .With("venue", "Park Wodny Kraków")
            .With("address", "ul. Dobrego Pasterza 126")
            .With("year_round", "true")
            .With("accessibility", "full")
            .With("opening_mon_thu", "10:00-22:00")
            .With("opening_fri", "10:00-23:00")
            .With("opening_sat", "09:00-23:00")
            .With("opening_sun", "09:00-22:00")
            .With("phone", "+48 12 616 31 90")
            .With("website", "parkwodny.pl");

    // ── Wawel helpers (below) ─────────────────────────────────────────────

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
