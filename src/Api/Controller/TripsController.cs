using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    [HttpGet("trips")]
    public async Task<ActionResult<List<Trip>>> GetAll()
    {
        return await _tripsService.GetAllAsync();
    }

    [HttpGet("trips/{id}")]
    public async Task<ActionResult<Trip>> GetById(string id)
    {
        var trip = await _tripsService.GetByIdAsync(id);
        if (trip == null)
            return NotFound();

        return trip;
    }

    [HttpGet("trips/route/{routeId}")]
    public async Task<ActionResult<List<Trip>>> GetByRouteId(string routeId)
    {
        return await _tripsService.GetByRouteIdAsync(routeId);
    }
}