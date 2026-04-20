using MediatR;
using Szlakomat.Products.Application.Relationships.Common;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.FindRelationsFrom;

public record FindRelationsFromQuery(
    IProductIdentifier From,
    ProductRelationshipType? Type
) : IRequest<IReadOnlyList<ProductRelationshipView>>;
