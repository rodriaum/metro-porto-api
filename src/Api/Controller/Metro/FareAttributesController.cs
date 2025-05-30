using MetroPorto.Api.Models;
using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.Metro;

[ApiController]
[Route("v1/porto/metro")]
public class FareAttributesController : ControllerBase
{
    private readonly IFareAttributesService _fareAttributesService;

    public FareAttributesController(IFareAttributesService fareAttributesService)
    {
        _fareAttributesService = fareAttributesService;
    }

    [HttpGet("fare-attributes")]
    public async Task<ActionResult<List<FareAttribute>>> GetAll()
    {
        return await _fareAttributesService.GetAllAsync();
    }

    [HttpGet("fare-attributes/{id}")]
    public async Task<ActionResult<FareAttribute>> GetById(string id)
    {
        FareAttribute? fareAttribute = await _fareAttributesService.GetByIdAsync(id);

        if (fareAttribute == null)
            return NotFound();

        return fareAttribute;
    }
}