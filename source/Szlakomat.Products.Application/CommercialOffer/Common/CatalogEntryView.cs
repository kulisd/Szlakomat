namespace Szlakomat.Products.Application.CommercialOffer.Common;

/// <summary>
/// View of a CatalogEntry - commercial offering position.
/// </summary>
public record CatalogEntryView(
    string CatalogEntryId,
    string DisplayName,
    string Description,
    string ProductTypeId,
    IReadOnlySet<string>? Categories,
    DateOnly? AvailableFrom,
    DateOnly? AvailableUntil,
    IReadOnlyDictionary<string, string>? Metadata
);
