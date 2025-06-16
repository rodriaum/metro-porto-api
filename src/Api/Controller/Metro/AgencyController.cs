using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.Metro;

[ApiController]
[Route("api/v1/transit/gtfs")]
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
        Agency? agency = await _agencyService.GetByIdAsync(id);

        if (agency == null)
            return NotFound();

        return agency;
    }
}