using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.Products.Api.Contracts.Products;
using Szlakomat.Products.Api.Mappers;
using Szlakomat.Products.Application.Catalog.DefineProductType;
using Szlakomat.Products.Application.Catalog.FindProductType;
using Szlakomat.Products.Application.Catalog.FindByTrackingStrategy;

namespace Szlakomat.Products.Api.Controllers;

/// <summary>
/// Zarządza definicjami typów produktów w bounded context Catalog.
/// Typy produktów są <b>niezmienialne po utworzeniu</b> — ten kontroler
/// udostępnia wyłącznie operacje definiowania i odczytu.
/// </summary>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Definiuje nowy typ produktu wraz z jego cechami wymaganymi, opcjonalnymi,
    /// jednostką miary i strategią śledzenia (indywidualne / partie / unikalne).
    /// </summary>
    /// <param name="req">Kompletna definicja typu produktu: identyfikator, nazwa, opis, jednostka, strategia śledzenia, cechy oraz metadane.</param>
    /// <response code="201">Typ produktu zdefiniowany — zwraca jego reprezentację oraz nagłówek <c>Location</c>.</response>
    /// <response code="400">Definicja odrzucona przez regułę domenową (np. duplikat identyfikatora, nieprawidłowa konfiguracja cech).</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductTypeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(DefineProductTypeRequest req)
    {
        var cmd = new DefineProductType(
            req.ProductIdType, req.ProductId, req.Name, req.Description,
            req.Unit, req.TrackingStrategy,
            req.MandatoryFeatures?.Select(ProductMapper.ToMandatoryFeature).ToHashSet(),
            req.OptionalFeatures?.Select(ProductMapper.ToOptionalFeature).ToHashSet(),
            req.Metadata);

        var result = await mediator.Send(cmd);
        if (!result.IsSuccess())
            return BadRequest(new { error = result.GetFailure() });

        var view = await mediator.Send(new FindProductTypeCriteria(req.ProductId));
        return view is not null
            ? CreatedAtAction(nameof(GetById), new { id = req.ProductId }, ProductMapper.ToResponse(view))
            : Problem();
    }

    /// <summary>
    /// Zwraca listę typów produktów filtrowanych po strategii śledzenia.
    /// Akceptuje formaty: CamelCase (IndividuallyTracked) lub UPPER_SNAKE_CASE (INDIVIDUALLY_TRACKED).
    /// </summary>
    /// <param name="trackingStrategy">Nazwa strategii śledzenia (np. <c>IndividuallyTracked</c>, <c>INDIVIDUALLY_TRACKED</c>, <c>BatchTracked</c>).</param>
    /// <response code="200">Kolekcja typów produktów spełniających kryterium (może być pusta).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? trackingStrategy)
    {
        if (string.IsNullOrWhiteSpace(trackingStrategy))
            return Ok(Array.Empty<ProductTypeResponse>());

        // Konwersja CamelCase na UPPER_SNAKE_CASE (np. IndividuallyTracked → INDIVIDUALLY_TRACKED)
        var normalizedStrategy = NormalizeCamelCaseToSnakeCase(trackingStrategy);
        
        var views = await mediator.Send(new FindByTrackingStrategyCriteria(normalizedStrategy));
        return Ok(views.Select(ProductMapper.ToResponse));
    }

    /// <summary>
    /// Konwertuje CamelCase na UPPER_SNAKE_CASE.
    /// Przykłady: 
    /// - "IndividuallyTracked" → "INDIVIDUALLY_TRACKED"
    /// - "BatchTracked" → "BATCH_TRACKED"
    /// - "Identical" → "IDENTICAL"
    /// </summary>
    private static string NormalizeCamelCaseToSnakeCase(string input)
    {
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c) && i > 0)
            {
                result.Append('_');
            }
            result.Append(char.ToUpperInvariant(c));
        }
        return result.ToString();
    }

    /// <summary>
    /// Pobiera pojedynczy typ produktu po jego identyfikatorze.
    /// </summary>
    /// <param name="id">Identyfikator typu produktu.</param>
    /// <response code="200">Znaleziony typ produktu.</response>
    /// <response code="404">Typ produktu o podanym identyfikatorze nie istnieje.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var view = await mediator.Send(new FindProductTypeCriteria(id));
        return view is not null ? Ok(ProductMapper.ToResponse(view)) : NotFound();
    }
}
