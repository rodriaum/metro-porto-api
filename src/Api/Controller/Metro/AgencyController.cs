using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.Metro;

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