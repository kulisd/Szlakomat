using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.Common;

internal static class ProductRelationshipMapper
{
    internal static ProductRelationshipView ToView(ProductRelationship rel) =>
        new(
            rel.Id.Value.ToString(),
            rel.From.ToString()!,
            rel.To.ToString()!,
            rel.Type.ToString().ToUpperInvariant());
}
