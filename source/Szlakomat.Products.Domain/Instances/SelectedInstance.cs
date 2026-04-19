using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// SelectedInstance represents a concrete instance (ProductInstance or PackageInstance)
/// that was selected/delivered as part of a package.
///
/// Used in PackageInstance to track what was actually delivered.
///
/// Example: Customer ordered "Laptop Bundle" and received:
/// - SelectedInstance(laptopInstance with serial ABC123, quantity=1)
/// - SelectedInstance(ramInstance from batch LOT-2025, quantity=1)
/// - SelectedInstance(ssdInstance with serial XYZ789, quantity=1)
/// </summary>
public record SelectedInstance
{
    public IInstance Instance { get; }
    public int Quantity { get; }

    public SelectedInstance(IInstance? instance, int quantity)
    {
        Guard.IsNotNull(instance);
        Guard.IsGreaterThan(quantity, 0);
        Instance = instance;
        Quantity = quantity;
    }

    /// <summary>
    /// Helper: get the Product (ProductType or PackageType) of this instance.
    /// </summary>
    public IProduct Product => Instance.Product();

    /// <summary>
    /// Helper: get the ProductIdentifier of this instance's product.
    /// </summary>
    public IProductIdentifier ProductId => Instance.Product().Id();

    /// <summary>
    /// Helper: get the InstanceId.
    /// </summary>
    public InstanceId InstanceId => Instance.Id();

    /// <summary>
    /// Converts this SelectedInstance to SelectedProduct.
    /// Useful for package validation where we need to check product types, not instances.
    /// </summary>
    public SelectedProduct ToSelectedProduct() =>
        new(Instance.Product().Id(), Quantity);
}
