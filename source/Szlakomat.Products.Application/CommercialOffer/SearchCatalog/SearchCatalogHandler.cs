using MediatR;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.SearchCatalog;

internal sealed class SearchCatalogHandler : IRequestHandler<SearchCatalogCriteria, IReadOnlySet<CatalogEntryView>>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public SearchCatalogHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlySet<CatalogEntryView>> Handle(SearchCatalogCriteria criteria, CancellationToken cancellationToken)
    {
        var result = _catalogRepository.FindAll()
            .Where(e => MatchesSearchText(e, criteria.SearchText) &&
                        MatchesCategories(e, criteria.Categories) &&
                        MatchesAvailability(e, criteria.AvailableAt) &&
                        MatchesProductType(e, criteria.ProductTypeId) &&
                        MatchesFeatures(e, criteria.ProductTypeFeatures))
            .Select(CatalogEntryMapper.ToCatalogEntryView)
            .ToHashSet();
        return Task.FromResult<IReadOnlySet<CatalogEntryView>>(result);
    }

    private static bool MatchesSearchText(CatalogEntry entry, string? searchText) =>
        searchText == null ||
        entry.DisplayName().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
        entry.Description().Contains(searchText, StringComparison.OrdinalIgnoreCase);

    private static bool MatchesCategories(CatalogEntry entry, IReadOnlySet<string>? categories) =>
        categories == null || categories.Count == 0 ||
        categories.Any(cat => entry.IsInCategory(cat));

    private static bool MatchesAvailability(CatalogEntry entry, DateOnly? availableAt) =>
        availableAt == null || entry.IsAvailableAt(availableAt.Value);

    private static bool MatchesProductType(CatalogEntry entry, string? productTypeId) =>
        productTypeId == null || entry.Product().Id().ToString() == productTypeId;

    private static bool MatchesFeatures(CatalogEntry entry, IReadOnlyDictionary<string, IReadOnlySet<string>>? features)
    {
        if (features == null || features.Count == 0) return true;
        if (entry.Product() is not ProductType productType) return false;

        var allFeatures = new HashSet<ProductFeatureType>(productType.FeatureTypes().MandatoryFeatures());
        allFeatures.UnionWith(productType.FeatureTypes().OptionalFeatures());

        foreach (var (featureName, requestedValues) in features)
        {
            var featureType = allFeatures.FirstOrDefault(f => f.Name == featureName);
            if (featureType == null) return false;

            bool anyValueMatches = requestedValues.Any(value => TryCastAndValidate(featureType, value));
            if (!anyValueMatches) return false;
        }
        return true;
    }

    private static bool TryCastAndValidate(ProductFeatureType featureType, string rawValue)
    {
        try
        {
            var typed = featureType.Constraint.ValueType.CastFrom(rawValue);
            return featureType.IsValidValue(typed);
        }
        catch
        {
            return false;
        }
    }
}
