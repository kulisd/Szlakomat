using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Catalog.PackageType;

/// <summary>
/// Represents a product selected by customer with quantity.
/// Used for validating package configuration against package structure rules.
/// </summary>
public record SelectedProduct
{
    public IProductIdentifier ProductId { get; }
    public int Quantity { get; }

    public SelectedProduct(IProductIdentifier? productId, int quantity)
    {
        Guard.IsNotNull(productId);
        Guard.IsGreaterThan(quantity, 0);
        ProductId = productId;
        Quantity = quantity;
    }
}
