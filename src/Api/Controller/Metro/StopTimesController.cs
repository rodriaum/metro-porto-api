using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPorto.Api.Controller.Metro;

[ApiController]
[Route("v1/porto/metro")]
public class StopTimesController : ControllerBase
{
    private readonly IStopTimesService _stopTimesService;

    public StopTimesController(IStopTimesService stopTimesService)
    {
        _stopTimesService = stopTimesService;
    }

    [HttpGet("stop-times")]
    public async Task<ActionResult<List<StopTime>>> GetAll()
    {
        return await _stopTimesService.GetAllAsync();
    }

    [HttpGet("stop-times/trip/{tripId}")]
    public async Task<ActionResult<List<StopTime>>> GetByTripId(string tripId)
    {
        return await _stopTimesService.GetByTripIdAsync(tripId);
    }

    [HttpGet("stop-times/stop/{stopId}")]
    public async Task<ActionResult<List<StopTime>>> GetByStopId(string stopId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
    {
        return await _stopTimesService.GetByStopIdAsync(stopId, page, pageSize);
    }
}