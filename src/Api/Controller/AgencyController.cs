using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
public class AgencyController : ControllerBase
{
    private readonly IAgencyService _agencyService;

    public AgencyController(IAgencyService agencyService)
    {
        _agencyService = agencyService;
    }

    [HttpGet("agencies")]
    public async Task<ActionResult<List<Agency>>> GetAll()
    {
        return await _agencyService.GetAllAsync();
    }

    [HttpGet("agencies/{id}")]
    public async Task<ActionResult<Agency>> GetById(string id)
    {
        var agency = await _agencyService.GetByIdAsync(id);
        if (agency == null)
            return NotFound();

        return agency;
    }
}