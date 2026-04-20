using Szlakomat.Products.Application.Catalog.DefineProductType;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Application.Tests.Assemblers;

/// <summary>
/// Assembler for DefineProductType commands — used only because of nested
/// MandatoryFeature / IFeatureConstraintConfig object graphs that are non-trivial to build inline.
/// </summary>
internal static class CatalogCommandAssembler
{
    /// <summary>
    /// Simple attraction with no variants — IDENTICAL tracking (every visit is the same experience).
    /// Example: Kosciuszko Mound — free entry, no guide, no options.
    /// </summary>
    public static DefineProductType SimpleAttraction(string id) =>
        new(
            "UUID", id,
            "Kosciuszko Mound",
            "Historic mound with panoramic view of Krakow — free, self-guided",
            "pcs",
            "IDENTICAL",
            null,
            null,
            new Dictionary<string, string> { ["city"] = "Krakow", ["type"] = "monument" });

    /// <summary>
    /// Attraction with mandatory guide language selection — BATCH_TRACKED (sold in tour groups).
    /// Example: Wawel Castle — guided tours in chosen language.
    /// </summary>
    public static DefineProductType GuidedAttraction(string id) =>
        new(
            "UUID", id,
            "Wawel Castle — Guided Tour",
            "Royal residence on Wawel Hill — group tours with licensed guide",
            "pcs",
            "BATCH_TRACKED",
            new HashSet<MandatoryFeature>
            {
                new("guide_language", new AllowedValuesConfig(
                    new HashSet<string> { "PL", "EN", "DE", "FR" }))
            },
            null,
            new Dictionary<string, string> { ["city"] = "Krakow", ["type"] = "castle" });

    /// <summary>
    /// Attraction with individually numbered reservations — INDIVIDUALLY_TRACKED.
    /// Example: Rynek Underground — timed entry slots with unique booking codes.
    /// </summary>
    public static DefineProductType TimedEntryAttraction(string id) =>
        new(
            "UUID", id,
            "Rynek Underground",
            "Interactive archaeological exhibition under Krakow's Main Square — timed entry",
            "pcs",
            "INDIVIDUALLY_TRACKED",
            null,
            null,
            new Dictionary<string, string> { ["city"] = "Krakow", ["type"] = "museum" });

    /// <summary>
    /// Attraction for organised groups — BATCH_TRACKED by group number.
    /// Example: National Museum — group visits tracked by booking batch.
    /// </summary>
    public static DefineProductType GroupAttraction(string id) =>
        new(
            "UUID", id,
            "National Museum in Krakow — Group Visit",
            "Permanent collection of the National Museum — group bookings",
            "pcs",
            "BATCH_TRACKED",
            null,
            null,
            new Dictionary<string, string> { ["city"] = "Krakow", ["type"] = "museum" });
}
