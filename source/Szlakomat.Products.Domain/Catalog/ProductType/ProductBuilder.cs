using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.SelectionRules;
using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Domain.Catalog.ProductType;

/// <summary>
/// Budowniczy do tworzenia zarówno ProductType jak i PackageType z płynnym API.
///
/// Wspólne atrybuty (id, nazwa, opis, metadane, stosowalność) są ustawiane w głównym budowniczym.
/// Atrybuty specyficzne dla typu są ustawiane w wyspecjalizowanych wewnętrznych budowniczych zwracanych przez AsProductType() lub AsPackageType().
///
/// Użycie:
/// <pre>
/// ProductType laptop = new ProductBuilder(id, name, description)
///     .WithMetadata("category", "electronics")
///     .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
///         .WithMandatoryFeature(colorFeature)
///         .Build();
///
/// PackageType bundle = new ProductBuilder(id, name, description)
///     .WithMetadata("promotion", "summer2025")
///     .AsPackageType()
///         .WithSingleChoice("Memory", ram8GB.Id(), ram16GB.Id())
///         .WithSingleChoice("Storage", ssd256GB.Id(), ssd512GB.Id())
///         .WithOptionalChoice("Accessories", mouse.Id(), bag.Id())
///         .Build();
/// </pre>
/// </summary>
internal class ProductBuilder
{
    // Wspólne pola - wymagane na wstępie
    private readonly IProductIdentifier _id;
    private readonly ProductName _name;
    private readonly ProductDescription _description;

    // Wspólne pola - opcjonalne z domyślnymi wartościami
    private ProductMetadata _metadata = ProductMetadata.Empty();
    private IApplicabilityConstraint _applicabilityConstraint = ApplicabilityConstraint.AlwaysTrue();

    internal ProductBuilder(IProductIdentifier id, ProductName name, ProductDescription description)
    {
        _id = id;
        _name = name;
        _description = description;
    }

    /// <summary>
    /// Ustawia metadane dla produktu/pakietu.
    /// </summary>
    public ProductBuilder WithMetadata(ProductMetadata metadata)
    {
        _metadata = metadata;
        return this;
    }

    /// <summary>
    /// Dodaje jeden wpis metadanych.
    /// </summary>
    public ProductBuilder WithMetadata(string key, string value)
    {
        _metadata = _metadata.With(key, value);
        return this;
    }

    /// <summary>
    /// Ustawia ograniczenie stosowalności dla produktu/pakietu.
    /// </summary>
    public ProductBuilder WithApplicabilityConstraint(IApplicabilityConstraint constraint)
    {
        _applicabilityConstraint = constraint;
        return this;
    }

    /// <summary>
    /// Rozpoczyna budowanie ProductType (zwykły produkt).
    /// Zwraca wyspecjalizowany budowniczy do atrybutów specyficznych ProductType.
    /// </summary>
    public ProductTypeBuilder AsProductType(Unit preferredUnit, ProductTrackingStrategy trackingStrategy)
    {
        return new ProductTypeBuilder(this, preferredUnit, trackingStrategy);
    }

    /// <summary>
    /// Rozpoczyna budowanie PackageType (pakiet produktów).
    /// Zwraca wyspecjalizowany budowniczy z płynnym API do definiowania struktury pakietu.
    /// </summary>
    public PackageTypeBuilder AsPackageType()
    {
        return new PackageTypeBuilder(this);
    }

    /// <summary>
    /// Wyspecjalizowany budowniczy do ProductType.
    /// </summary>
    public class ProductTypeBuilder
    {
        private readonly ProductBuilder _parent;
        private readonly Unit _preferredUnit;
        private readonly ProductTrackingStrategy _trackingStrategy;
        private readonly List<ProductFeatureTypeDefinition> _featureDefinitions = new();

        public ProductTypeBuilder(ProductBuilder parent, Unit preferredUnit, ProductTrackingStrategy trackingStrategy)
        {
            _parent = parent;
            _preferredUnit = preferredUnit;
            _trackingStrategy = trackingStrategy;
        }

        public ProductTypeBuilder WithMandatoryFeature(ProductFeatureType featureType)
        {
            _featureDefinitions.Add(ProductFeatureTypeDefinition.MakeMandatory(featureType));
            return this;
        }

        public ProductTypeBuilder WithOptionalFeature(ProductFeatureType featureType)
        {
            _featureDefinitions.Add(ProductFeatureTypeDefinition.MakeOptional(featureType));
            return this;
        }

        public ProductTypeBuilder WithFeature(ProductFeatureTypeDefinition definition)
        {
            _featureDefinitions.Add(definition);
            return this;
        }

        public ProductTypeBuilder WithApplicabilityConstraint(IApplicabilityConstraint constraint)
        {
            _parent._applicabilityConstraint = constraint;
            return this;
        }

        public ProductTypeBuilder WithMetadata(ProductMetadata metadata)
        {
            _parent._metadata = metadata;
            return this;
        }

        public ProductTypeBuilder WithMetadata(string key, string value)
        {
            _parent._metadata = _parent._metadata.With(key, value);
            return this;
        }

        public ProductType Build()
        {
            ProductFeatureTypes features = new ProductFeatureTypes(_featureDefinitions);
            return new ProductType(
                _parent._id,
                _parent._name,
                _parent._description,
                _preferredUnit,
                _trackingStrategy,
                features,
                _parent._metadata,
                _parent._applicabilityConstraint);
        }
    }

    /// <summary>
    /// Specialized builder for PackageType with fluent API for defining package structure.
    /// </summary>
    public class PackageTypeBuilder
    {
        private readonly ProductBuilder _parent;
        private readonly Dictionary<string, ProductSet> _productSets = new();
        private readonly List<ISelectionRule> _selectionRules = new();
        private ProductTrackingStrategy _trackingStrategy = ProductTrackingStrategy.IndividuallyTracked;

        public PackageTypeBuilder(ProductBuilder parent)
        {
            _parent = parent;
        }

        public PackageTypeBuilder WithTrackingStrategy(ProductTrackingStrategy trackingStrategy)
        {
            _trackingStrategy = trackingStrategy;
            return this;
        }

        /// <summary>
        /// Adds a product set with "select exactly one" rule.
        /// Example: customer must choose exactly 1 memory option from the set.
        /// </summary>
        public PackageTypeBuilder WithSingleChoice(string setName, params IProductIdentifier[] productIds)
        {
            return WithChoice(setName, 1, 1, productIds);
        }

        /// <summary>
        /// Adds a product set with "select zero or one" rule.
        /// Example: customer may optionally choose 1 accessory from the set.
        /// </summary>
        public PackageTypeBuilder WithOptionalChoice(string setName, params IProductIdentifier[] productIds)
        {
            return WithChoice(setName, 0, 1, productIds);
        }

        /// <summary>
        /// Adds a product set with "select at least one" rule.
        /// Example: customer must choose at least 1 item from the set (can choose more).
        /// </summary>
        public PackageTypeBuilder WithRequiredChoice(string setName, params IProductIdentifier[] productIds)
        {
            return WithChoice(setName, 1, int.MaxValue, productIds);
        }

        /// <summary>
        /// Adds a product set with custom min/max selection rule.
        /// Example: customer must choose 2-4 items from the set.
        /// </summary>
        public PackageTypeBuilder WithChoice(string setName, int min, int max, params IProductIdentifier[] productIds)
        {
            ProductSet set = new ProductSet(setName, productIds);
            _productSets[setName] = set;
            _selectionRules.Add(SelectionRule.IsSubsetOf(set, min, max));
            return this;
        }

        public PackageTypeBuilder WithProductSet(string setName, params IProductIdentifier[] productIds)
        {
            return WithProductSet(new ProductSet(setName, productIds));
        }

        public PackageTypeBuilder WithProductSet(ProductSet set)
        {
            _productSets[set.Name] = set;
            return this;
        }

        public PackageTypeBuilder WithProductSets(params ProductSet[] productSets)
        {
            foreach (var set in productSets)
            {
                _productSets[set.Name] = set;
            }
            return this;
        }

        /// <summary>
        /// Adds a custom selection rule for advanced cases (conditional rules, AND/OR composition).
        /// Use this when simple choice rules are not enough.
        /// </summary>
        public PackageTypeBuilder WithRule(ISelectionRule rule)
        {
            _selectionRules.Add(rule);
            return this;
        }

        /// <summary>
        /// Returns the current ProductSet by name for use in conditional rules.
        /// </summary>
        public ProductSet GetProductSet(string setName) => _productSets[setName];

        public PackageTypeBuilder WithApplicabilityConstraint(IApplicabilityConstraint constraint)
        {
            _parent._applicabilityConstraint = constraint;
            return this;
        }

        public PackageTypeBuilder WithMetadata(ProductMetadata metadata)
        {
            _parent._metadata = metadata;
            return this;
        }

        public PackageTypeBuilder WithMetadata(string key, string value)
        {
            _parent._metadata = _parent._metadata.With(key, value);
            return this;
        }

        public PackageType.PackageType Build()
        {
            PackageStructure structure = new PackageStructure(_productSets, _selectionRules);
            return new PackageType.PackageType(
                _parent._id,
                _parent._name,
                _parent._description,
                _trackingStrategy,
                _parent._metadata,
                _parent._applicabilityConstraint,
                structure);
        }
    }
}
