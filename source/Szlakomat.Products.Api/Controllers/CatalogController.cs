using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.Products.Api.Contracts.Catalog;
using Szlakomat.Products.Api.Mappers;
using Szlakomat.Products.Application.CommercialOffer.AddToOffer;
using Szlakomat.Products.Application.CommercialOffer.FindCatalogEntry;
using Szlakomat.Products.Application.CommercialOffer.SearchCatalog;
using Szlakomat.Products.Application.CommercialOffer.UpdateMetadata;
using Szlakomat.Products.Application.CommercialOffer.DiscontinueProduct;

namespace Szlakomat.Products.Api.Controllers;

/// <summary>
/// Zarządza wpisami w katalogu oferty handlowej (CommercialOffer) — publikacją
/// typów produktów, wyszukiwaniem, aktualizacją metadanych oraz wycofaniem
/// produktu ze sprzedaży.
/// </summary>
[ApiController]
[Route("api/catalog")]
[Produces("application/json")]
public class CatalogController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Dodaje istniejący typ produktu do katalogu oferty handlowej.
    /// </summary>
    /// <param name="req">Dane wpisu katalogowego: identyfikator typu produktu,
    /// nazwa wyświetlana, opis, kategorie, okres dostępności oraz metadane.</param>
    /// <response code="201">Wpis katalogowy utworzony — zwraca jego reprezentację oraz nagłówek <c>Location</c>.</response>
    /// <response code="400">Żądanie odrzucone przez regułę domenową (np. nieznany typ produktu, nieprawidłowy okres).</response>
    [HttpPost]
    [ProducesResponseType(typeof(CatalogEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToOffer(AddToOfferRequest req)
    {
        var cmd = new AddToOffer(
            req.ProductTypeId, req.DisplayName, req.Description,
            req.Categories?.ToHashSet(),
            req.AvailableFrom != null ? DateOnly.Parse(req.AvailableFrom) : null,
            req.AvailableUntil != null ? DateOnly.Parse(req.AvailableUntil) : null,
            req.Metadata);

        var result = await mediator.Send(cmd);
        if (!result.IsSuccess()) return BadRequest(new { error = result.GetFailure() });

        var id = result.SuccessValue.Value;
        var view = await mediator.Send(new FindCatalogEntryCriteria(id));
        return view is not null
            ? CreatedAtAction(nameof(GetById), new { id }, CatalogMapper.ToResponse(view))
            : Problem();
    }

    /// <summary>
    /// Wyszukuje wpisy katalogowe według frazy, kategorii, daty dostępności
    /// lub identyfikatora typu produktu.
    /// </summary>
    /// <param name="q">Opcjonalna fraza dopasowywana do nazwy lub opisu wpisu.</param>
    /// <param name="category">Opcjonalny filtr kategorii (pojedyncza wartość).</param>
    /// <param name="availableAt">Opcjonalna data (ISO 8601), na którą wpis ma być aktywny.</param>
    /// <param name="productTypeId">Opcjonalny identyfikator typu produktu, do którego wpis ma się odwoływać.</param>
    /// <response code="200">Kolekcja wpisów katalogowych spełniających kryteria (może być pusta).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CatalogEntryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] string? category,
        [FromQuery] string? availableAt,
        [FromQuery] string? productTypeId)
    {
        var criteria = new SearchCatalogCriteria(
            q,
            category != null ? new HashSet<string> { category } : null,
            availableAt != null ? DateOnly.Parse(availableAt) : null,
            productTypeId,
            null);
        var views = await mediator.Send(criteria);
        return Ok(views.Select(CatalogMapper.ToResponse));
    }

    /// <summary>
    /// Pobiera pojedynczy wpis katalogowy po jego identyfikatorze.
    /// </summary>
    /// <param name="id">Identyfikator wpisu katalogowego.</param>
    /// <response code="200">Znaleziony wpis katalogowy.</response>
    /// <response code="404">Wpis o podanym identyfikatorze nie istnieje.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CatalogEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var view = await mediator.Send(new FindCatalogEntryCriteria(id));
        return view is not null ? Ok(CatalogMapper.ToResponse(view)) : NotFound();
    }

    /// <summary>
    /// Aktualizuje metadane (pary klucz–wartość) istniejącego wpisu katalogowego.
    /// </summary>
    /// <param name="id">Identyfikator wpisu katalogowego.</param>
    /// <param name="req">Nowy zestaw metadanych, który zastąpi dotychczasowy.</param>
    /// <response code="200">Zwraca zaktualizowany wpis katalogowy.</response>
    /// <response code="400">Aktualizacja odrzucona przez regułę domenową.</response>
    /// <response code="404">Wpis o podanym identyfikatorze nie istnieje.</response>
    [HttpPatch("{id}/metadata")]
    [ProducesResponseType(typeof(CatalogEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMetadata(string id, UpdateMetadataRequest req)
    {
        if (await mediator.Send(new FindCatalogEntryCriteria(id)) is null)
            return NotFound();

        var result = await mediator.Send(new UpdateMetadata(id, req.Metadata));
        if (!result.IsSuccess()) return BadRequest(new { error = result.GetFailure() });

        return Ok(CatalogMapper.ToResponse((await mediator.Send(new FindCatalogEntryCriteria(id)))!));
    }

    /// <summary>
    /// Wycofuje produkt ze sprzedaży z podaną datą zakończenia dostępności.
    /// </summary>
    /// <param name="id">Identyfikator wpisu katalogowego.</param>
    /// <param name="req">Data wycofania produktu (ISO 8601).</param>
    /// <response code="200">Zwraca wpis katalogowy z ustawioną datą wycofania.</response>
    /// <response code="400">Operacja odrzucona przez regułę domenową (np. data w przeszłości).</response>
    /// <response code="404">Wpis o podanym identyfikatorze nie istnieje.</response>
    [HttpPatch("{id}/discontinue")]
    [ProducesResponseType(typeof(CatalogEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Discontinue(string id, DiscontinueRequest req)
    {
        if (await mediator.Send(new FindCatalogEntryCriteria(id)) is null)
            return NotFound();

        var result = await mediator.Send(new DiscontinueProduct(id, DateOnly.Parse(req.DiscontinuationDate)));
        if (!result.IsSuccess()) return BadRequest(new { error = result.GetFailure() });

        return Ok(CatalogMapper.ToResponse((await mediator.Send(new FindCatalogEntryCriteria(id)))!));
    }
}
