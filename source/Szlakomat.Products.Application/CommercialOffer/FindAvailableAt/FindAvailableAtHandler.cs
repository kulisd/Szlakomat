using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindAvailableAt;

internal sealed class FindAvailableAtHandler : IRequestHandler<FindAvailableAtCriteria, IReadOnlySet<CatalogEntryView>>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public FindAvailableAtHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlySet<CatalogEntryView>> Handle(FindAvailableAtCriteria request, CancellationToken cancellationToken)
    {
        var result = _catalogRepository.FindAll()
            .Where(e => e.IsAvailableAt(request.Date))
            .Select(CatalogEntryMapper.ToCatalogEntryView)
            .ToHashSet();
        return Task.FromResult<IReadOnlySet<CatalogEntryView>>(result);
    }
}
