namespace Szlakomat.Products.Domain.Common.Identifiers;

/// <summary>
/// Prosta implementacja ProductIdentifier oparta na UUID.
/// Używaj gdy nie potrzebujesz interoperacyjności z zewnętrznymi systemami standardowymi (ISBN, GTIN itp.)
/// </summary>
public record UuidProductIdentifier(Guid Value) : IProductIdentifier
{
    public static UuidProductIdentifier New() => new(Guid.NewGuid());

    public static UuidProductIdentifier Of(string value) => new(Guid.Parse(value));

    public string Type() => "UUID";

    public override string ToString() => Value.ToString();
}
