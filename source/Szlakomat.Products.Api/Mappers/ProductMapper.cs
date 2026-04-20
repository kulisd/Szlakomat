using System.Globalization;
using System.Text.Json;
using Szlakomat.Products.Api.Contracts.Products;
using Szlakomat.Products.Application.Catalog.Common;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.FeatureConstraints;

namespace Szlakomat.Products.Api.Mappers;

internal static class ProductMapper
{
    internal static ProductTypeResponse ToResponse(ProductTypeView v) =>
        new(v.ProductId, v.Name, v.Description, v.Unit, v.TrackingStrategy,
            v.MandatoryFeatures?.Select(f =>
                new FeatureTypeDetail(f.Name, f.ValueType, f.ConstraintType, f.ConstraintDescription)).ToList(),
            v.OptionalFeatures?.Select(f =>
                new FeatureTypeDetail(f.Name, f.ValueType, f.ConstraintType, f.ConstraintDescription)).ToList());

    internal static MandatoryFeature ToMandatoryFeature(FeatureDefinition f) =>
        new(f.Name, ToConstraintConfig(f));

    internal static OptionalFeature ToOptionalFeature(FeatureDefinition f) =>
        new(f.Name, ToConstraintConfig(f));

    private static IFeatureConstraintConfig ToConstraintConfig(FeatureDefinition f) =>
        f.ConstraintType.ToUpper() switch
        {
            "ALLOWED_VALUES" => new AllowedValuesConfig(ParseSet(f.ConstraintParams, "allowedValues")),
            "NUMERIC_RANGE"  => new NumericRangeConfig(
                                    ParseInt(f.ConstraintParams, "min"), ParseInt(f.ConstraintParams, "max")),
            "DECIMAL_RANGE"  => new DecimalRangeConfig(
                                    ParseStr(f.ConstraintParams, "min"), ParseStr(f.ConstraintParams, "max")),
            "REGEX"          => new RegexConfig(ParseStr(f.ConstraintParams, "pattern")),
            "DATE_RANGE"     => new DateRangeConfig(
                                    ParseStr(f.ConstraintParams, "from"), ParseStr(f.ConstraintParams, "to")),
            _                => new UnconstrainedConfig(
                                    ParseStr(f.ConstraintParams, "valueType", fallback: "TEXT"))
        };

    private static IReadOnlySet<string> ParseSet(IReadOnlyDictionary<string, object>? p, string key)
    {
        if (p is null || !p.TryGetValue(key, out var raw) || raw is null)
            return new HashSet<string>();

        if (raw is JsonElement json)
        {
            if (json.ValueKind != JsonValueKind.Array)
                return new HashSet<string>();
            return json.EnumerateArray()
                .Select(e => e.ValueKind == JsonValueKind.String ? e.GetString() ?? "" : e.ToString())
                .ToHashSet();
        }

        if (raw is IEnumerable<object> enumerable)
            return enumerable.Select(v => v?.ToString() ?? "").ToHashSet();

        return new HashSet<string>();
    }

    private static int ParseInt(IReadOnlyDictionary<string, object>? p, string key)
    {
        if (p is null || !p.TryGetValue(key, out var raw) || raw is null)
            return 0;

        if (raw is JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.Number => json.GetInt32(),
                JsonValueKind.String => int.Parse(json.GetString() ?? "0", CultureInfo.InvariantCulture),
                _ => 0
            };
        }

        return Convert.ToInt32(raw, CultureInfo.InvariantCulture);
    }

    private static string ParseStr(IReadOnlyDictionary<string, object>? p, string key, string fallback = "")
    {
        if (p is null || !p.TryGetValue(key, out var raw) || raw is null)
            return fallback;

        if (raw is JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.String => json.GetString() ?? fallback,
                JsonValueKind.Null or JsonValueKind.Undefined => fallback,
                _ => json.ToString()
            };
        }

        return raw.ToString() ?? fallback;
    }
}
