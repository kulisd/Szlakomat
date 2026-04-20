namespace Szlakomat.Products.Domain.Common.Applicability;

/// <summary>
/// Kontekst do oceny ograniczeń stosowalności.
/// Ogólna mapa klucz-wartość reprezentująca sytuację, w której sprawdzamy stosowalność.
/// </summary>
public class ApplicabilityContext
{
    private readonly IReadOnlyDictionary<string, string> _parameters;

    private ApplicabilityContext(IReadOnlyDictionary<string, string> parameters)
    {
        _parameters = new Dictionary<string, string>(parameters);
    }

    public static ApplicabilityContext Empty() => new(new Dictionary<string, string>());

    public static ApplicabilityContext Of(Dictionary<string, string>? parameters) =>
        new(parameters ?? new Dictionary<string, string>());

    public string? Get(string key) => _parameters.TryGetValue(key, out var value) ? value : null;

    public string GetOrDefault(string key, string defaultValue) =>
        Get(key) ?? defaultValue;

    public bool Has(string key) => _parameters.ContainsKey(key);

    public IReadOnlyDictionary<string, string> AsMap() => _parameters;

    public override string ToString() => $"ApplicabilityContext{_parameters}";
}
