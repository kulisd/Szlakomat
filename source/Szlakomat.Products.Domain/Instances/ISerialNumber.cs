namespace Szlakomat.Products.Domain.Instances;

public interface ISerialNumber
{
    string Type { get; }
    string Value { get; }

    static ISerialNumber Of(string value) => TextualSerialNumber.Of(value);

    static ISerialNumber Vin(string value) => VinSerialNumber.Of(value);

    static ISerialNumber Imei(string value) => ImeiSerialNumber.Of(value);
}
