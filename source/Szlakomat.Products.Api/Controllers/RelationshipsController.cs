using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.Products.Api.Contracts.Relationships;
using Szlakomat.Products.Api.Mappers;
using Szlakomat.Products.Application.Relationships.DefineRelationship;
using Szlakomat.Products.Application.Relationships.FindRelationsFrom;
using Szlakomat.Products.Application.Relationships.RemoveRelationship;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Api.Controllers;

/// <summary>
/// Zarządza skierowanymi relacjami między typami produktów (bounded context
/// Relationships). Dozwolone rodzaje relacji są kontrolowane przez
/// <c>IProductRelationshipDefiningPolicy</c>.
/// </summary>
[ApiController]
[Route("api/relationships")]
[Produces("application/json")]
public class RelationshipsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Definiuje nową skierowaną relację między dwoma typami produktów.
    /// </summary>
    /// <param name="req">Identyfikator typu źródłowego, identyfikator typu docelowego oraz nazwa typu relacji.</param>
    /// <response code="201">Relacja utworzona — zwraca jej reprezentację oraz nagłówek <c>Location</c>.</response>
    /// <response code="400">Żądanie odrzucone przez politykę domenową (np. niedozwolony typ relacji, nieznany typ produktu, duplikat relacji).</response>
    [HttpPost]
    [ProducesResponseType(typeof(RelationshipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Define(DefineRelationshipRequest req)
    {
        var result = await mediator.Send(
            new DefineRelationship(
                req.FromIdentifierType, req.FromProductId,
                req.ToIdentifierType, req.ToProductId,
                req.RelationshipType));
        return result.IsSuccess()
            ? Created($"/api/relationships/{result.SuccessValue.Value}",
                new RelationshipResponse(
                    result.SuccessValue.Value.ToString(),
                    req.FromProductId, req.ToProductId, req.RelationshipType))
            : BadRequest(new { error = result.GetFailure() });
    }

    /// <summary>
    /// Zwraca relacje wychodzące z danego typu produktu, opcjonalnie zawężone
    /// do wskazanego rodzaju relacji.
    /// </summary>
    /// <param name="fromProductId">Identyfikator typu produktu, dla którego zwracane są relacje wychodzące.</param>
    /// <param name="type">Opcjonalny filtr — nazwa typu relacji.</param>
    /// <response code="200">Kolekcja relacji spełniających kryteria (może być pusta).</response>
    /// <response code="400">Identyfikator typu źródłowego ma nieprawidłowy format.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List([FromQuery] string fromProductId, [FromQuery] string? type)
    {
        if (!Guid.TryParse(fromProductId, out _))
            return BadRequest(new { error = $"Invalid fromProductId: '{fromProductId}'. Expected a GUID value." });

        var from = UuidProductIdentifier.Of(fromProductId);
        var rels = await mediator.Send(new FindRelationsFromQuery(from, type != null ? RelationshipMapper.ParseType(type) : null));
        return Ok(rels.Select(RelationshipMapper.ToResponse));
    }

    /// <summary>
    /// Usuwa istniejącą relację między typami produktów.
    /// </summary>
    /// <param name="id">Identyfikator relacji.</param>
    /// <response code="204">Relacja została usunięta.</response>
    /// <response code="400">Identyfikator relacji ma nieprawidłowy format.</response>
    /// <response code="404">Relacja o podanym identyfikatorze nie istnieje.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest(new { error = $"Invalid relationship id: '{id}'. Expected a GUID value." });

        var result = await mediator.Send(new RemoveRelationship(id));
        if (result.IsSuccess())
            return NoContent();

        var failure = result.GetFailure();
        return failure != null && failure.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? NotFound(new { error = failure })
            : BadRequest(new { error = failure });
    }
}
