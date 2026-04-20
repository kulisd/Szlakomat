using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Application.CommercialOffer.UpdateMetadata;

internal sealed class UpdateMetadataHandler : IRequestHandler<UpdateMetadata, Result<string, CatalogEntryId>>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public UpdateMetadataHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<Result<string, CatalogEntryId>> Handle(UpdateMetadata command, CancellationToken cancellationToken)
    {
        var entryId = CatalogEntryId.Of(command.CatalogEntryId);
        var entry = _catalogRepository.FindById(entryId);
        if (entry == null)
            return Task.FromResult(Result<string, CatalogEntryId>.FailureOf($"Catalog entry not found: {command.CatalogEntryId}"));

        var updated = entry.WithMetadata(command.Metadata);
        _catalogRepository.Save(updated);
        return Task.FromResult(Result<string, CatalogEntryId>.SuccessOf(entryId));
    }
}
