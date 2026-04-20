using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.FeatureConstraints;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Application.Catalog.Common;

internal static class ProductTypeMapper
{
    internal static IProductIdentifier ParseProductIdentifier(string type, string value) => type.ToUpper() switch
    {
        "UUID" => UuidProductIdentifier.Of(value),
        "ISBN" => IsbnProductIdentifier.Of(value),
        "GTIN" => GtinProductIdentifier.Of(value),
        "INSPIRE" => InspireProductIdentifier.Of(value),
        _ => throw new ArgumentException($"Unknown product identifier type: {type}")
    };

    internal static ProductTrackingStrategy ParseTrackingStrategy(string strategy) => strategy.ToUpper() switch
    {
        "IDENTICAL" => ProductTrackingStrategy.Identical,
        "INDIVIDUALLY_TRACKED" => ProductTrackingStrategy.IndividuallyTracked,
        "BATCH_TRACKED" => ProductTrackingStrategy.BatchTracked,
        "INDIVIDUALLY_AND_BATCH_TRACKED" => ProductTrackingStrategy.IndividuallyAndBatchTracked,
        "UNIQUE" => ProductTrackingStrategy.Unique,
        _ => throw new ArgumentException($"Unknown tracking strategy: {strategy}")
    };

    internal static Unit ParseUnit(string symbol) => symbol.ToLowerInvariant() switch
    {
        "pcs" or "pieces" => Unit.Pieces(),
        "kg" or "kilograms" => Unit.Kilograms(),
        "l" or "liters" => Unit.Liters(),
        "m" or "meters" => Unit.Meters(),
        "m²" or "m2" or "square meters" => Unit.SquareMeters(),
        "m³" or "m3" or "cubic meters" => Unit.CubicMeters(),
        "h" or "hours" => Unit.Hours(),
        "min" or "minutes" => Unit.Minutes(),
        _ => Unit.Of(symbol, symbol)
    };

    private static FeatureValueType ParseFeatureValueType(string valueType) => valueType.ToUpper() switch
    {
        "TEXT" => FeatureValueType.Text,
        "INTEGER" => FeatureValueType.Integer,
        "DECIMAL" => FeatureValueType.Decimal,
        "DATE" => FeatureValueType.Date,
        "BOOLEAN" => FeatureValueType.Boolean,
        _ => throw new ArgumentException($"Unknown feature value type: {valueType}")
    };

    internal static IFeatureValueConstraint ToConstraint(IFeatureConstraintConfig config) => config switch
    {
        AllowedValuesConfig c => AllowedValuesConstraint.Of(c.AllowedValues.ToArray()),
        NumericRangeConfig c => new NumericRangeConstraint(c.Min, c.Max),
        DecimalRangeConfig c => DecimalRangeConstraint.Of(c.Min, c.Max),
        RegexConfig c => new RegexConstraint(c.Pattern),
        DateRangeConfig c => DateRangeConstraint.Between(c.From, c.To),
        UnconstrainedConfig c => new Unconstrained(ParseFeatureValueType(c.ValueType)),
        _ => throw new ArgumentException($"Unknown constraint config type: {config.GetType().Name}")
    };

    internal static Result<string, IProductIdentifier> TryParseProductIdentifier(string type, string value) =>
        type?.ToUpper() switch
        {
            "UUID" => Result<string, IProductIdentifier>.SuccessOf(UuidProductIdentifier.Of(value)),
            "ISBN" => Result<string, IProductIdentifier>.SuccessOf(IsbnProductIdentifier.Of(value)),
            "GTIN" => Result<string, IProductIdentifier>.SuccessOf(GtinProductIdentifier.Of(value)),
            "INSPIRE" => Result<string, IProductIdentifier>.SuccessOf(InspireProductIdentifier.Of(value)),
            _ => Result<string, IProductIdentifier>.FailureOf($"Unknown product identifier type: {type}")
        };

    internal static Result<string, ProductTrackingStrategy> TryParseTrackingStrategy(string strategy) =>
        strategy?.ToUpper() switch
        {
            "IDENTICAL" => Result<string, ProductTrackingStrategy>.SuccessOf(ProductTrackingStrategy.Identical),
            "INDIVIDUALLY_TRACKED" => Result<string, ProductTrackingStrategy>.SuccessOf(ProductTrackingStrategy.IndividuallyTracked),
            "BATCH_TRACKED" => Result<string, ProductTrackingStrategy>.SuccessOf(ProductTrackingStrategy.BatchTracked),
            "INDIVIDUALLY_AND_BATCH_TRACKED" => Result<string, ProductTrackingStrategy>.SuccessOf(ProductTrackingStrategy.IndividuallyAndBatchTracked),
            "UNIQUE" => Result<string, ProductTrackingStrategy>.SuccessOf(ProductTrackingStrategy.Unique),
            _ => Result<string, ProductTrackingStrategy>.FailureOf($"Unknown tracking strategy: {strategy}")
        };

    internal static Result<string, IFeatureValueConstraint> TryToConstraint(IFeatureConstraintConfig config) => config switch
    {
        AllowedValuesConfig c => Result<string, IFeatureValueConstraint>.SuccessOf(AllowedValuesConstraint.Of(c.AllowedValues.ToArray())),
        NumericRangeConfig c => Result<string, IFeatureValueConstraint>.SuccessOf(new NumericRangeConstraint(c.Min, c.Max)),
        DecimalRangeConfig c => Result<string, IFeatureValueConstraint>.SuccessOf(DecimalRangeConstraint.Of(c.Min, c.Max)),
        RegexConfig c => Result<string, IFeatureValueConstraint>.SuccessOf(new RegexConstraint(c.Pattern)),
        DateRangeConfig c => Result<string, IFeatureValueConstraint>.SuccessOf(DateRangeConstraint.Between(c.From, c.To)),
        UnconstrainedConfig c => TryParseFeatureValueType(c.ValueType)
            .Map<IFeatureValueConstraint>(vt => new Unconstrained(vt)),
        _ => Result<string, IFeatureValueConstraint>.FailureOf($"Unknown constraint config type: {config.GetType().Name}")
    };

    private static Result<string, FeatureValueType> TryParseFeatureValueType(string valueType) =>
        valueType?.ToUpper() switch
        {
            "TEXT" => Result<string, FeatureValueType>.SuccessOf(FeatureValueType.Text),
            "INTEGER" => Result<string, FeatureValueType>.SuccessOf(FeatureValueType.Integer),
            "DECIMAL" => Result<string, FeatureValueType>.SuccessOf(FeatureValueType.Decimal),
            "DATE" => Result<string, FeatureValueType>.SuccessOf(FeatureValueType.Date),
            "BOOLEAN" => Result<string, FeatureValueType>.SuccessOf(FeatureValueType.Boolean),
            _ => Result<string, FeatureValueType>.FailureOf($"Unknown feature value type: {valueType}")
        };

    private static IReadOnlyDictionary<string, object>? ConstraintConfigToMap(IFeatureValueConstraint constraint) =>
        constraint switch
        {
            AllowedValuesConstraint c => new Dictionary<string, object> { { "allowedValues", c.GetAllowedValues() } },
            NumericRangeConstraint c => new Dictionary<string, object> { { "min", c.Min }, { "max", c.Max } },
            DecimalRangeConstraint c => new Dictionary<string, object> { { "min", c.Min }, { "max", c.Max } },
            RegexConstraint c => new Dictionary<string, object> { { "pattern", c.Pattern } },
            DateRangeConstraint c => new Dictionary<string, object> { { "from", c.From.ToString() }, { "to", c.To.ToString() } },
            Unconstrained => new Dictionary<string, object>(),
            _ => null
        };

    private static string ToTrackingStrategyString(ProductTrackingStrategy strategy) => strategy switch
    {
        ProductTrackingStrategy.Identical => "IDENTICAL",
        ProductTrackingStrategy.IndividuallyTracked => "INDIVIDUALLY_TRACKED",
        ProductTrackingStrategy.BatchTracked => "BATCH_TRACKED",
        ProductTrackingStrategy.IndividuallyAndBatchTracked => "INDIVIDUALLY_AND_BATCH_TRACKED",
        ProductTrackingStrategy.Unique => "UNIQUE",
        _ => strategy.ToString().ToUpperInvariant()
    };

    private static string ToValueTypeString(FeatureValueType valueType) => valueType switch
    {
        FeatureValueType.Text => "TEXT",
        FeatureValueType.Integer => "INTEGER",
        FeatureValueType.Decimal => "DECIMAL",
        FeatureValueType.Date => "DATE",
        FeatureValueType.Boolean => "BOOLEAN",
        _ => valueType.ToString().ToUpperInvariant()
    };

    private static FeatureTypeView ToFeatureTypeView(ProductFeatureType featureType) =>
        new(
            featureType.Name,
            ToValueTypeString(featureType.Constraint.ValueType),
            featureType.Constraint.Type,
            ConstraintConfigToMap(featureType.Constraint),
            featureType.Constraint.Desc);

    internal static ProductTypeView ToProductTypeView(ProductType pt) =>
        new(
            pt.Id().ToString()!,
            pt.Name().ToString(),
            pt.Description().ToString(),
            pt.PreferredUnit().Symbol,
            ToTrackingStrategyString(pt.TrackingStrategy()),
            pt.FeatureTypes().MandatoryFeatures().Select(ToFeatureTypeView).ToHashSet(),
            pt.FeatureTypes().OptionalFeatures().Select(ToFeatureTypeView).ToHashSet());
}
