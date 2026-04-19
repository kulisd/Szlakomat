using MediatR;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Application.CommercialOffer.Common;

namespace Szlakomat.Products.Application.CommercialOffer.AddToOffer;

internal sealed class AddToOfferHandler : IRequestHandler<AddToOffer, Result<string, CatalogEntryId>>
{
    private readonly ICatalogEntryRepository _catalogRepository;
    private readonly IProductTypeRepository _productTypeRepository;

    public AddToOfferHandler(ICatalogEntryRepository catalogRepository, IProductTypeRepository productTypeRepository)
    {
        _catalogRepository = catalogRepository;
        _productTypeRepository = productTypeRepository;
    }

    public Task<Result<string, CatalogEntryId>> Handle(AddToOffer command, CancellationToken cancellationToken)
    {
        var productType = _productTypeRepository.FindByIdValue(command.ProductTypeId);
        if (productType == null)
            return Task.FromResult(Result<string, CatalogEntryId>.FailureOf($"Product type not found: {command.ProductTypeId}"));

        var validity = CatalogEntryMapper.BuildValidity(command.AvailableFrom, command.AvailableUntil);
        var id = CatalogEntryId.Generate();

        var builder = CatalogEntry.CreateBuilder()
            .WithId(id)
            .WithDisplayName(command.DisplayName)
            .WithDescription(command.Description)
            .WithProduct(productType)
            .WithValidity(validity);

        if (command.Categories != null)
            builder = builder.WithCategories(command.Categories);

        if (command.Metadata != null)
            builder = builder.WithMetadata(new Dictionary<string, string>(command.Metadata));

        var entry = builder.Build();
        _catalogRepository.Save(entry);
        return Task.FromResult(Result<string, CatalogEntryId>.SuccessOf(id));
    }
}
