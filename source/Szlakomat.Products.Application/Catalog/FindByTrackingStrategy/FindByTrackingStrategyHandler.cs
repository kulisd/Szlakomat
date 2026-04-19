using MediatR;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Application.Catalog.Common;

namespace Szlakomat.Products.Application.Catalog.FindByTrackingStrategy;

internal sealed class FindByTrackingStrategyHandler : IRequestHandler<FindByTrackingStrategyCriteria, IReadOnlySet<ProductTypeView>>
{
    private readonly IProductTypeRepository _repository;

    public FindByTrackingStrategyHandler(IProductTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlySet<ProductTypeView>> Handle(FindByTrackingStrategyCriteria request, CancellationToken cancellationToken)
    {
        var strategy = ProductTypeMapper.ParseTrackingStrategy(request.TrackingStrategy);
        var result = _repository.FindByTrackingStrategy(strategy)
            .Select(ProductTypeMapper.ToProductTypeView)
            .ToHashSet();
        return Task.FromResult<IReadOnlySet<ProductTypeView>>(result);
    }
}
