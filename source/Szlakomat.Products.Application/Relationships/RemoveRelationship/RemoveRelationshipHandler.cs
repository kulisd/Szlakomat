using MediatR;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.RemoveRelationship;

internal sealed class RemoveRelationshipHandler : IRequestHandler<RemoveRelationship, Result<string, ProductRelationshipId>>
{
    private readonly IProductRelationshipRepository _repository;

    public RemoveRelationshipHandler(IProductRelationshipRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<string, ProductRelationshipId>> Handle(RemoveRelationship command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(command.RelationshipId, out var guid))
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf(
                $"Invalid relationship id: '{command.RelationshipId}'. Expected a GUID value."));

        var id = ProductRelationshipId.Of(guid);
        var deleted = _repository.Delete(id);
        if (deleted is null)
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf($"Relationship not found: {command.RelationshipId}"));
        return Task.FromResult(Result<string, ProductRelationshipId>.SuccessOf(deleted));
    }
}
