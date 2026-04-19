using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// PackageInstance reprezentuje konkretny egzemplarz PackageType z dostarczonymi egzemplarzami.
///
/// Przykłady:
/// - PackageType: "Pakiet laptopa" -> PackageInstance: dostarczony laptop SN-ABC, RAM partia-001, SSD SN-XYZ
/// - PackageType: "Pakiet bankowy" -> PackageInstance: dostarczona karta Gold SN-123, polisa ubezpieczenia POL-456
///
/// PackageInstance śledzi:
/// - Który PackageType został kupiony
/// - Konkretne egzemplarze dostarczone (ProductInstance lub PackageInstance)
/// - Numer seryjny i/lub identyfikator partii dla samego pakietu — wymagane jest co najmniej jedno z nich
///
/// Dostarczone egzemplarze są walidowane względem struktury PackageType, aby upewnić się, że wszystkie reguły są spełnione.
/// </summary>
internal class PackageInstance : IInstance
{
    private readonly InstanceId _id;
    private readonly PackageType _packageType;
    private readonly IReadOnlyList<SelectedInstance> _selection;
    private readonly ISerialNumber? _serialNumber;
    private readonly BatchId? _batchId;

    public PackageInstance(
        InstanceId? id,
        PackageType? packageType,
        List<SelectedInstance>? selection,
        ISerialNumber? serialNumber,
        BatchId? batchId)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(packageType);
        Guard.IsNotEmpty(selection);

        ValidateTrackingRequirements(packageType, serialNumber, batchId);
        ValidateSelection(packageType, selection!);

        _id = id;
        _packageType = packageType;
        _selection = selection!.AsReadOnly();
        _serialNumber = serialNumber;
        _batchId = batchId;
    }

    private static void ValidateTrackingRequirements(
        PackageType packageType,
        ISerialNumber? serialNumber,
        BatchId? batchId)
    {
        Guard.IsTrue(
            serialNumber != null || batchId != null,
            "PackageInstance must have either SerialNumber or BatchId (or both)");

        ProductTrackingStrategy strategy = packageType.TrackingStrategy();

        if (strategy.IsTrackedIndividually() && serialNumber == null)
        {
            throw new ArgumentException(
                $"PackageType requires individual tracking (strategy: {strategy}) but no serial number defined");
        }

        if (strategy.IsTrackedByBatch() && batchId == null)
        {
            throw new ArgumentException(
                $"PackageType requires batch tracking (strategy: {strategy}) but no batch id defined");
        }

        if (strategy.RequiresBothTrackingMethods() && (serialNumber == null || batchId == null))
        {
            throw new ArgumentException(
                $"PackageType requires both individual and batch tracking (strategy: {strategy})");
        }
    }

    private static void ValidateSelection(PackageType packageType, List<SelectedInstance> selection)
    {
        // Konwertuj SelectedInstance na SelectedProduct do walidacji względem reguł pakietu
        var selectedProducts = selection
            .Select(si => si.ToSelectedProduct())
            .ToList();

        PackageValidationResult result = packageType.ValidateSelection(selectedProducts);
        if (!result.IsValid)
        {
            throw new ArgumentException(
                $"Invalid package selection: {string.Join(", ", result.Errors)}");
        }
    }

    public InstanceId Id() => _id;

    public IProduct Product() => _packageType;

    public PackageType PackageType() => _packageType;

    public IReadOnlyList<SelectedInstance> Selection() => _selection;

    public ISerialNumber? SerialNumber() => _serialNumber;

    public BatchId? BatchId() => _batchId;

    public override string ToString()
    {
        var serialStr = _serialNumber != null ? _serialNumber.ToString() : "none";
        var batchStr = _batchId != null ? _batchId.ToString() : "none";
        return $"PackageInstance{{id={_id}, type={_packageType.Name()}, serial={serialStr}, batch={batchStr}, selection={_selection.Count} products}}";
    }
}
