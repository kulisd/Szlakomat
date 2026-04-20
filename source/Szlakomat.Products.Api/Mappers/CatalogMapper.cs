using Szlakomat.Products.Api.Contracts.Catalog;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Api.Mappers;

internal static class CatalogMapper
{
    internal static CatalogEntryResponse ToResponse(CatalogEntryView v) =>
        new(v.CatalogEntryId, v.DisplayName, v.Description, v.ProductTypeId,
            v.Categories?.ToList(),
            v.AvailableFrom?.ToString("yyyy-MM-dd"),
            v.AvailableUntil?.ToString("yyyy-MM-dd"),
            v.Metadata);
}
