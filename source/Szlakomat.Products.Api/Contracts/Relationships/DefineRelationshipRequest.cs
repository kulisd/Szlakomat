namespace Szlakomat.Products.Api.Contracts.Relationships;

public record DefineRelationshipRequest(
    string FromIdentifierType,
    string FromProductId,
    string ToIdentifierType,
    string ToProductId,
    string RelationshipType
);
