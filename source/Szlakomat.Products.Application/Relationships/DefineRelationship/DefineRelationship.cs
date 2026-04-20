using MediatR;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.DefineRelationship;

/// <summary>
/// Command to define a directed relationship between two products.
/// </summary>
public record DefineRelationship(
    string FromIdentifierType, // "UUID", "ISBN", "GTIN", "INSPIRE"
    string FromProductId,
    string ToIdentifierType,
    string ToProductId,
    string RelationshipType // "UPGRADABLE_TO", "SUBSTITUTED_BY", etc.
) : IRequest<Result<string, ProductRelationshipId>>;
