using MediatR;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Application.CommercialOffer.DiscontinueProduct;

internal sealed class DiscontinueProductHandler : IRequestHandler<DiscontinueProduct, Result<string, CatalogEntryId>>
{
    private readonly ICatalogEntryRepository _catalogRepository;

    public DiscontinueProductHandler(ICatalogEntryRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<Result<string, CatalogEntryId>> Handle(DiscontinueProduct command, CancellationToken cancellationToken)
    {
        var entryId = CatalogEntryId.Of(command.CatalogEntryId);
        var entry = _catalogRepository.FindById(entryId);
        if (entry == null)
            return Task.FromResult(Result<string, CatalogEntryId>.FailureOf($"Catalog entry not found: {command.CatalogEntryId}"));

        var discontinuationDate = command.DiscontinuationDate;
        var newValidity = entry.Validity().From != null
            ? Validity.Between(entry.Validity().From!.Value, discontinuationDate)
            : Validity.Until(discontinuationDate);

        var updated = entry.WithValidity(newValidity);
        _catalogRepository.Save(updated);
        return Task.FromResult(Result<string, CatalogEntryId>.SuccessOf(entryId));
    }
}
