using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
public class StopsController : ControllerBase
{
    private readonly IStopsService _stopsService;

    public StopsController(IStopsService stopsService)
    {
        _stopsService = stopsService;
    }

    [HttpGet("stops")]
    public async Task<ActionResult<List<Stop>>> GetAll()
    {
        return await _stopsService.GetAllAsync();
    }

    [HttpGet("stops/{id}")]
    public async Task<ActionResult<Stop>> GetById(string id)
    {
        var stop = await _stopsService.GetByIdAsync(id);
        if (stop == null)
            return NotFound();

        return stop;
    }
}