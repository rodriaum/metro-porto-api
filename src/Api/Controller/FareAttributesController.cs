using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
        var fareAttribute = await _fareAttributesService.GetByIdAsync(id);
        if (fareAttribute == null)
            return NotFound();

        return fareAttribute;
    }
}