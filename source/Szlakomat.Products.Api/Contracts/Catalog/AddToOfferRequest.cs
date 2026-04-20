namespace Szlakomat.Products.Api.Contracts.Catalog;

public record AddToOfferRequest(
    string ProductTypeId,
    string DisplayName,
    string Description,
    List<string>? Categories,
    string? AvailableFrom,
    string? AvailableUntil,
    Dictionary<string, string>? Metadata
);
