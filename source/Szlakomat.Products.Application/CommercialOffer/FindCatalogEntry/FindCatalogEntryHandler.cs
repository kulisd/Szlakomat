using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindCatalogEntry;

internal sealed class FindCatalogEntryHandler : IRequestHandler<FindCatalogEntryCriteria, CatalogEntryView?>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public FindCatalogEntryHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<CatalogEntryView?> Handle(FindCatalogEntryCriteria request, CancellationToken cancellationToken)
    {
        var entry = _catalogRepository.FindById(CatalogEntryId.Of(request.CatalogEntryId));
        return Task.FromResult(entry != null ? CatalogEntryMapper.ToCatalogEntryView(entry) : null);
    }
}
