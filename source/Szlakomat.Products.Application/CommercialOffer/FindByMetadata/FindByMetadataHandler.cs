using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.FindByMetadata;

internal sealed class FindByMetadataHandler : IRequestHandler<FindByMetadataCriteria, IReadOnlySet<CatalogEntryView>>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public FindByMetadataHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlySet<CatalogEntryView>> Handle(FindByMetadataCriteria request, CancellationToken cancellationToken)
    {
        var result = _catalogRepository.FindAll()
            .Where(e => e.HasMetadata(request.Key) &&
                        (request.Value == null || e.GetMetadata(request.Key) == request.Value))
            .Select(CatalogEntryMapper.ToCatalogEntryView)
            .ToHashSet();
        return Task.FromResult<IReadOnlySet<CatalogEntryView>>(result);
    }
}
