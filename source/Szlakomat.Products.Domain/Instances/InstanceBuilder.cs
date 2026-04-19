using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Instances;

using QuantityVal = Quantity.Quantity;

/// <summary>
/// Budowniczy do tworzenia zarówno ProductInstance jak i PackageInstance z płynnym API.
/// </summary>
internal class InstanceBuilder
{
    private readonly InstanceId _id;
    private ISerialNumber? _serialNumber;
    private BatchId? _batchId;

    internal InstanceBuilder(InstanceId id)
    {
        _id = id;
    }

    public InstanceBuilder WithSerial(ISerialNumber serialNumber)
    {
        _serialNumber = serialNumber;
        return this;
    }

    public InstanceBuilder WithBatch(BatchId batchId)
    {
        _batchId = batchId;
        return this;
    }

    public ProductInstanceBuilder AsProductInstance(ProductType productType) => new(this, productType);

    public PackageInstanceBuilder AsPackageInstance(PackageType packageType) => new(this, packageType);

    public class ProductInstanceBuilder
    {
        private readonly InstanceBuilder _parent;
        private readonly ProductType _productType;
        private QuantityVal? _quantity;
        private readonly List<ProductFeatureInstance> _features = new();

        public ProductInstanceBuilder(InstanceBuilder parent, ProductType productType)
        {
            _parent = parent;
            _productType = productType;
        }

        public ProductInstanceBuilder WithQuantity(QuantityVal quantity)
        {
            _quantity = quantity;
            return this;
        }

        public ProductInstanceBuilder WithFeature(ProductFeatureInstance feature)
        {
            _features.Add(feature);
            return this;
        }

        public ProductInstanceBuilder WithFeature(ProductFeatureType featureType, object? value)
        {
            _features.Add(new ProductFeatureInstance(featureType, value));
            return this;
        }

        public ProductInstance Build()
        {
            var featureInstances = new ProductFeatureInstances(_features);
            return new ProductInstance(
                _parent._id,
                _productType,
                _parent._serialNumber,
                _parent._batchId,
                _quantity,
                featureInstances);
        }
    }

    public class PackageInstanceBuilder
    {
        private readonly InstanceBuilder _parent;
        private readonly PackageType _packageType;
        private List<SelectedInstance>? _selection;

        public PackageInstanceBuilder(InstanceBuilder parent, PackageType packageType)
        {
            _parent = parent;
            _packageType = packageType;
        }

        public PackageInstanceBuilder WithSelection(List<SelectedInstance> selection)
        {
            _selection = selection;
            return this;
        }

        public PackageInstance Build()
            => new(
                _parent._id,
                _packageType,
                _selection ?? new List<SelectedInstance>(),
                _parent._serialNumber,
                _parent._batchId);
    }
}
