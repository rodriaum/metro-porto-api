using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
public class TransfersController : ControllerBase
{
    private readonly ITransfersService _transfersService;

    public TransfersController(ITransfersService transfersService)
    {
        _transfersService = transfersService;
    }

    [HttpGet("transfers")]
    public async Task<ActionResult<List<Transfer>>> GetAll()
    {
        return await _transfersService.GetAllAsync();
    }

    [HttpGet("transfers/from/{fromStopId}")]
    public async Task<ActionResult<List<Transfer>>> GetByFromStopId(string fromStopId)
    {
        return await _transfersService.GetByFromStopIdAsync(fromStopId);
    }
}