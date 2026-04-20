using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindByCategory;

internal sealed class FindByCategoryHandler : IRequestHandler<FindByCategoryCriteria, IReadOnlySet<CatalogEntryView>>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public FindByCategoryHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlySet<CatalogEntryView>> Handle(FindByCategoryCriteria request, CancellationToken cancellationToken)
    {
        var result = _catalogRepository.FindByCategory(request.Category)
            .Select(CatalogEntryMapper.ToCatalogEntryView)
            .ToHashSet();
        return Task.FromResult<IReadOnlySet<CatalogEntryView>>(result);
    }
}
