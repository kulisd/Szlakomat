using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.Products.Api.Contracts.Instances;
using Szlakomat.Products.Api.Mappers;
using Szlakomat.Products.Application.Instances.CreateProductInstance;
using Szlakomat.Products.Application.Instances.CreatePackageInstance;
using Szlakomat.Products.Application.Instances.FindInstance;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Api.Controllers;

/// <summary>
/// Obsługuje tworzenie oraz pobieranie konkretnych instancji produktów
/// i pakietów (bounded context Instances). Instancje reprezentują egzemplarze
/// typów zdefiniowanych w katalogu — egzemplarze indywidualne, partie lub pakiety.
/// </summary>
[ApiController]
[Route("api/instances")]
[Produces("application/json")]
public class InstancesController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Tworzy nową instancję produktu (liść w wzorcu Composite).
    /// Strategia śledzenia oraz wartości cech są weryfikowane w momencie tworzenia.
    /// </summary>
    /// <param name="req">Identyfikator typu produktu, opcjonalny numer seryjny lub identyfikator
    /// partii, ilość wraz z jednostką oraz wartości cech wymaganych/opcjonalnych.</param>
    /// <response code="201">Instancja utworzona — zwraca jej reprezentację oraz nagłówek <c>Location</c>.</response>
    /// <response code="400">Żądanie odrzucone przez regułę domenową (np. naruszenie strategii śledzenia, nieznany typ produktu, niedozwolona wartość cechy).</response>
    [HttpPost("product")]
    [ProducesResponseType(typeof(InstanceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(CreateProductInstanceRequest req)
    {
        var cmd = new CreateProductInstance(
            req.ProductTypeId,
            req.SerialNumber,
            req.BatchId,
            req.Quantity,
            req.Unit,
            req.Features?.Select(f => new FeatureInstanceConfig(f.FeatureName, f.Value)).ToHashSet());

        var result = await mediator.Send(cmd);
        if (!result.IsSuccess())
            return BadRequest(new { error = result.GetFailure() });

        var instanceId = result.SuccessValue.ToString();
        var view = await mediator.Send(new FindInstanceQuery(instanceId));
        return view is not null
            ? CreatedAtAction(nameof(GetById), new { id = instanceId }, InstanceMapper.ToResponse(view))
            : Problem();
    }

    /// <summary>
    /// Tworzy nową instancję pakietu (composite w wzorcu Composite) na podstawie
    /// wybranego zbioru instancji składowych. Reguły selekcji pakietu są
    /// egzekwowane w momencie tworzenia.
    /// </summary>
    /// <param name="req">Identyfikator typu pakietu, opcjonalne numer seryjny lub partia oraz lista wybranych instancji wraz z ich ilościami.</param>
    /// <response code="201">Instancja pakietu utworzona — zwraca jej reprezentację oraz nagłówek <c>Location</c>.</response>
    /// <response code="400">Selekcja niezgodna z regułami pakietu (np. brak wymaganego elementu, przekroczenie dopuszczalnej liczby elementów).</response>
    [HttpPost("package")]
    [ProducesResponseType(typeof(InstanceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePackage(CreatePackageInstanceRequest req)
    {
        var cmd = new CreatePackageInstance(
            req.PackageTypeId,
            req.SerialNumber,
            req.BatchId,
            req.Selection.Select(s => new SelectedInstanceConfig(s.InstanceId, s.Quantity)).ToHashSet());

        var result = await mediator.Send(cmd);
        if (!result.IsSuccess())
            return BadRequest(new { error = result.GetFailure() });

        var instanceId = result.SuccessValue.ToString();
        var view = await mediator.Send(new FindInstanceQuery(instanceId));
        return view is not null
            ? CreatedAtAction(nameof(GetById), new { id = instanceId }, InstanceMapper.ToResponse(view))
            : Problem();
    }

    /// <summary>
    /// Pobiera instancję produktu lub pakietu po jej identyfikatorze.
    /// </summary>
    /// <param name="id">Identyfikator instancji.</param>
    /// <response code="200">Znaleziona instancja.</response>
    /// <response code="404">Instancja o podanym identyfikatorze nie istnieje.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InstanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var view = await mediator.Send(new FindInstanceQuery(id));
        return view is not null ? Ok(InstanceMapper.ToResponse(view)) : NotFound();
    }
}
