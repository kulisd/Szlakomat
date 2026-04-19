using MediatR;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.RemoveRelationship;

/// <summary>
/// Command to remove a product relationship by its identifier.
/// </summary>
public record RemoveRelationship(
    string RelationshipId
) : IRequest<Result<string, ProductRelationshipId>>;
