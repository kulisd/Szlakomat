using Szlakomat.Products.Api.Contracts.Relationships;
using Szlakomat.Products.Application.Relationships.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Api.Mappers;

internal static class RelationshipMapper
{
    internal static RelationshipResponse ToResponse(ProductRelationshipView view) =>
        new(view.Id, view.FromProductId, view.ToProductId, view.RelationshipType);

    internal static ProductRelationshipType ParseType(string type) => type.ToUpper() switch
    {
        "UPGRADABLE_TO"     => ProductRelationshipType.UpgradableTo,
        "SUBSTITUTED_BY"    => ProductRelationshipType.SubstitutedBy,
        "REPLACED_BY"       => ProductRelationshipType.ReplacedBy,
        "COMPLEMENTED_BY"   => ProductRelationshipType.ComplementedBy,
        "COMPATIBLE_WITH"   => ProductRelationshipType.CompatibleWith,
        "INCOMPATIBLE_WITH" => ProductRelationshipType.IncompatibleWith,
        _ => throw new ArgumentException($"Unknown relationship type: {type}")
    };
}
