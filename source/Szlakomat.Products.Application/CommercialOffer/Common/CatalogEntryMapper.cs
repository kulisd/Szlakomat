using Szlakomat.Products.Domain.CommercialOffer;

namespace Szlakomat.Products.Application.CommercialOffer.Common;

internal static class CatalogEntryMapper
{
    internal static CatalogEntryView ToCatalogEntryView(CatalogEntry entry) =>
        new(
            entry.Id().Value,
            entry.DisplayName(),
            entry.Description(),
            entry.Product().Id().ToString()!,
            entry.Categories(),
            entry.Validity().From,
            entry.Validity().To,
            entry.Metadata());

    internal static Validity BuildValidity(DateOnly? from, DateOnly? to) => (from, to) switch
    {
        ({ } f, { } t) => Validity.Between(f, t),
        ({ } f, null) => Validity.FromDate(f),
        (null, { } t) => Validity.Until(t),
        _ => Validity.Always()
    };
}
