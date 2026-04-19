namespace Szlakomat.Products.Api.Contracts.Relationships;

public record RelationshipResponse(
    string Id,
    string FromProductId,
    string ToProductId,
    string RelationshipType
);
