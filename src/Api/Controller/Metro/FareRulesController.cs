using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPorto.Api.Controller.Metro;

[ApiController]
[Route("v1/porto/metro")]
public class FareRulesController : ControllerBase
{
    private readonly IFareRulesService _fareRulesService;

    public FareRulesController(IFareRulesService fareRulesService)
    {
        _fareRulesService = fareRulesService;
    }

    [HttpGet("fare-rules")]
    public async Task<ActionResult<List<FareRule>>> GetAll()
    {
        return await _fareRulesService.GetAllAsync();
    }

    [HttpGet("fare-rules/fare/{fareId}")]
    public async Task<ActionResult<List<FareRule>>> GetByFareId(string fareId)
    {
        List<FareRule>? fareRule = await _fareRulesService.GetByFareIdAsync(fareId);

        if (fareRule == null)
            return NotFound();

        return fareRule;
    }
}