using MediatR;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.SearchCatalog;

/// <summary>
/// Criteria to search catalog entries.
/// All filters are optional - null/empty values are ignored.
/// </summary>
public record SearchCatalogCriteria(
    string? SearchText, // Search in displayName and description
    IReadOnlySet<string>? Categories, // Filter by categories (any match)
    DateOnly? AvailableAt, // Filter by availability at specific date
    string? ProductTypeId, // Filter by specific ProductType
    IReadOnlyDictionary<string, IReadOnlySet<string>>?
        ProductTypeFeatures // Filter by ProductType features and their possible values
    // Example: {"storage": ["256GB", "512GB"], "color": ["Blue"]}
    // Matches if ProductType has feature "storage" that accepts "256GB" OR "512GB"
) : IRequest<IReadOnlySet<CatalogEntryView>>
{
    /// <summary>
    /// Creates criteria for full catalog listing (no filters).
    /// </summary>
    public static SearchCatalogCriteria All() =>
        new(null, null, null, null, null);

    /// <summary>
    /// Creates criteria for text search only.
    /// </summary>
    public static SearchCatalogCriteria ByText(string searchText) =>
        new(searchText, null, null, null, null);

    /// <summary>
    /// Creates criteria for category filter only.
    /// </summary>
    public static SearchCatalogCriteria ByCategories(IReadOnlySet<string> categories) =>
        new(null, categories, null, null, null);

    /// <summary>
    /// Creates criteria for availability at specific date.
    /// </summary>
    public static SearchCatalogCriteria AvailableAtDate(DateOnly date) =>
        new(null, null, date, null, null);

    /// <summary>
    /// Creates criteria for specific product type.
    /// </summary>
    public static SearchCatalogCriteria ByProductType(string productTypeId) =>
        new(null, null, null, productTypeId, null);
}
