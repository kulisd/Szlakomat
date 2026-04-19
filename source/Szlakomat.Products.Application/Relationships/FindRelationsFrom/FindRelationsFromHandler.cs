using MediatR;
using Szlakomat.Products.Application.Relationships.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.FindRelationsFrom;

internal sealed class FindRelationsFromHandler : IRequestHandler<FindRelationsFromQuery, IReadOnlyList<ProductRelationshipView>>
{
    private readonly IProductRelationshipRepository _repository;

    public FindRelationsFromHandler(IProductRelationshipRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ProductRelationshipView>> Handle(FindRelationsFromQuery request, CancellationToken cancellationToken)
    {
        var relationships = request.Type.HasValue
            ? _repository.FindAllRelationsFrom(request.From, request.Type.Value)
            : _repository.FindAllRelationsFrom(request.From);

        IReadOnlyList<ProductRelationshipView> result = relationships
            .Select(ProductRelationshipMapper.ToView)
            .ToList();
        return Task.FromResult(result);
    }
}
