using MediatR;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Application.Catalog.Common;

namespace Szlakomat.Products.Application.Catalog.FindProductType;

internal sealed class FindProductTypeHandler : IRequestHandler<FindProductTypeCriteria, ProductTypeView?>
{
    private readonly IProductTypeRepository _repository;

    public FindProductTypeHandler(IProductTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<ProductTypeView?> Handle(FindProductTypeCriteria request, CancellationToken cancellationToken)
    {
        var productType = _repository.FindByIdValue(request.ProductId);
        return Task.FromResult(productType != null ? ProductTypeMapper.ToProductTypeView(productType) : null);
    }
}
