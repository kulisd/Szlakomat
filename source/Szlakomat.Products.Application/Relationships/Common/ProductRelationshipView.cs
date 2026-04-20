namespace Szlakomat.Products.Application.Relationships.Common;

/// <summary>
/// Read-only projection of ProductRelationship exposed by query handlers.
/// </summary>
public record ProductRelationshipView(
    string Id,
    string FromProductId,
    string ToProductId,
    string RelationshipType
);
