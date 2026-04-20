namespace Szlakomat.Products.Domain.Relationships;

public record ProductRelationshipId(Guid Value)
{
    public static ProductRelationshipId Of(Guid value) => new(value);

    public static ProductRelationshipId Random() => new(Guid.NewGuid());

    public string AsString() => Value.ToString();
}
