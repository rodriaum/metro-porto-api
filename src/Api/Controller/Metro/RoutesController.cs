using MetroPorto.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.Metro;

[ApiController]
[Route("v1/porto/metro")]
public class RoutesController : ControllerBase
{
    private readonly IRoutesService _routesService;

    public RoutesController(IRoutesService routesService)
    {
        _routesService = routesService;
    }

    [HttpGet("routes")]
    public async Task<ActionResult<List<Models.Route>>> GetAll()
    {
        return await _routesService.GetAllAsync();
    }

    [HttpGet("routes/{id}")]
    public async Task<ActionResult<Models.Route>> GetById(string id)
    {
        var route = await _routesService.GetByIdAsync(id);
        if (route == null)
            return NotFound();

        return route;
    }
}