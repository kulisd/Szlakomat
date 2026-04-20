using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Instance - konkretna realizacja fizyczna lub usługowa Produktu.
/// Może być albo ProductInstance (liść) albo PackageInstance (kompozyt).
/// </summary>
public interface IInstance
{
    InstanceId Id();
    IProduct Product();
    ISerialNumber? SerialNumber();
    BatchId? BatchId();
}
