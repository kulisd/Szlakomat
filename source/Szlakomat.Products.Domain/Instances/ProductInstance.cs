using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Domain.Instances;

using QuantityVal = Quantity.Quantity;

/// <summary>
/// ProductInstance reprezentuje konkretny egzemplarz/realizację ProductType.
///
/// Przykłady:
/// - ProductType: "iPhone 15 Pro 256GB" -> ProductInstance: konkretny telefon z numerem seryjnym ABC123
/// - ProductType: "Czysty kod (książka)" -> ProductInstance: konkretna kopia książki
/// - ProductType: "Mleko organiczne 1L" -> ProductInstance: konkretna butelka z partii LOT-2024-001
/// - ProductType: "Konsultacje" -> ProductInstance: 8,5 godzin świadczonych konsultacji
///
/// Każdy egzemplarz musi być śledzony przynajmniej jednym z:
/// - SerialNumber (śledzenie indywidualne)
/// - Batch (śledzenie grupowe do kontroli jakości)
/// - Lub obydwoma (np. drogi sprzęt w partiach jak telewizory)
/// </summary>
internal class ProductInstance : IInstance
{
    private readonly InstanceId _id;
    private readonly ProductType _productType;
    private readonly ISerialNumber? _serialNumber;
    private readonly BatchId? _batchId;
    private readonly QuantityVal? _quantity;
    private readonly ProductFeatureInstances _features;

    public ProductInstance(
        InstanceId? id,
        ProductType? productType,
        ISerialNumber? serialNumber,
        BatchId? batchId,
        QuantityVal? quantity,
        ProductFeatureInstances? features)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(productType);
        Guard.IsNotNull(features);

        ValidateTrackingRequirements(productType, serialNumber, batchId);
        ValidateQuantityUnit(productType, quantity);
        features.ValidateAgainst(productType.FeatureTypes());

        _id = id;
        _productType = productType;
        _serialNumber = serialNumber;
        _batchId = batchId;
        _quantity = quantity;
        _features = features;
    }

    private static void ValidateTrackingRequirements(
        ProductType productType,
        ISerialNumber? serialNumber,
        BatchId? batchId)
    {
        ProductTrackingStrategy strategy = productType.TrackingStrategy();

        if (strategy.IsInterchangeable())
        {
            Guard.IsTrue(
                serialNumber == null && batchId == null,
                "IDENTICAL products must not have SerialNumber or BatchId");
            return;
        }

        Guard.IsTrue(
            serialNumber != null || batchId != null,
            $"ProductInstance must have either SerialNumber or BatchId for strategy: {strategy}");

        if (strategy.IsTrackedIndividually() && serialNumber == null)
            throw new ArgumentException($"ProductType requires individual tracking (strategy: {strategy}) but no serial number defined");

        if (strategy.IsTrackedByBatch() && batchId == null)
            throw new ArgumentException($"ProductType requires batch tracking (strategy: {strategy}) but no batch id defined");

        if (strategy.RequiresBothTrackingMethods() && (serialNumber == null || batchId == null))
            throw new ArgumentException($"ProductType requires both individual and batch tracking (strategy: {strategy}) but neither serial number nor batch id defined");
    }

    private static void ValidateQuantityUnit(ProductType productType, QuantityVal? quantity)
    {
        if (quantity != null)
        {
            Guard.IsTrue(
                quantity.Unit.Equals(productType.PreferredUnit()),
                "Quantity unit must match ProductType's preferred unit");
        }
    }

    public InstanceId Id() => _id;

    public IProduct Product() => _productType;

    public ProductType ProductType() => _productType;

    public ISerialNumber? SerialNumber() => _serialNumber;

    public BatchId? BatchId() => _batchId;

    public QuantityVal? Quantity() => _quantity;

    /// <summary>
    /// Zwraca efektywną ilość tego egzemplarza.
    /// Jeśli jest ustawiona jawna ilość, zwraca ją.
    /// W przeciwnym razie zwraca niejawne "1 jednostka" preferowanej jednostki produktu.
    /// </summary>
    public QuantityVal EffectiveQuantity() =>
        _quantity ?? QuantityVal.Of(1, _productType.PreferredUnit());

    public ProductFeatureInstances Features() => _features;

    public override string ToString()
    {
        var serialStr = _serialNumber?.ToString() ?? "none";
        var batchStr = _batchId?.ToString() ?? "none";
        var quantityStr = _quantity?.ToString() ?? $"implicit 1 {_productType.PreferredUnit()}";
        return $"ProductInstance{{id={_id}, type={_productType.Name()}, serial={serialStr}, batch={batchStr}, quantity={quantityStr}, features={_features}}}";
    }
}
