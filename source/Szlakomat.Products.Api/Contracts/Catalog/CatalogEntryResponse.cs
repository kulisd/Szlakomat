namespace Szlakomat.Products.Api.Contracts.Catalog;

public record CatalogEntryResponse(
    string CatalogEntryId,
    string DisplayName,
    string Description,
    string ProductTypeId,
    List<string>? Categories,
    string? AvailableFrom,
    string? AvailableUntil,
    IReadOnlyDictionary<string, string>? Metadata
);
