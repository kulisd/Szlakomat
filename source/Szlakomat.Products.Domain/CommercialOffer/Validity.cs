namespace Szlakomat.Products.Domain.CommercialOffer;

/// <summary>
/// Reprezentuje okres czasu, w którym coś jest ważne (dostępne, aktywne itp.).
/// Obie granice są opcjonalne:
/// - Brak daty "od" = ważne od początku czasów
/// - Brak daty "do" = ważne na zawsze
/// </summary>
internal class Validity
{
    public DateOnly? From { get; }
    public DateOnly? To { get; }

    private Validity(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new ArgumentException("From date must be before or equal to to date");
        }
        From = from;
        To = to;
    }

    /// <summary>
    /// Tworzy okres ważności od danej daty (włącznie) bez daty końcowej.
    /// </summary>
    public static Validity FromDate(DateOnly from) => new(from, null);

    /// <summary>
    /// Tworzy okres ważności do danej daty (włącznie) bez daty początkowej.
    /// </summary>
    public static Validity Until(DateOnly to) => new(null, to);

    /// <summary>
    /// Tworzy okres ważności między dwiema datami (obie włącznie).
    /// </summary>
    public static Validity Between(DateOnly from, DateOnly to) => new(from, to);

    /// <summary>
    /// Tworzy okres ważności bez granic (zawsze ważne).
    /// </summary>
    public static Validity Always() => new(null, null);

    /// <summary>
    /// Sprawdza czy dana data przypada w tym okresie ważności.
    /// </summary>
    public bool IsValidAt(DateOnly date)
    {
        if (From.HasValue && date < From.Value)
            return false;
        if (To.HasValue && date > To.Value)
            return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        Validity validity = (Validity)obj;
        return From == validity.From && To == validity.To;
    }

    public override int GetHashCode() => HashCode.Combine(From, To);

    public override string ToString() => (From, To) switch
    {
        (null, null) => "always",
        (null, _) => $"until {To}",
        (_, null) => $"from {From}",
        _ => $"{From} to {To}"
    };
}
