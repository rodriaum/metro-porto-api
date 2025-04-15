using MetroPorto.Api.Filter;
using MetroPorto.Api.Models;
using MetroPorto.Api.Interfaces.Gtfs;
using Microsoft.AspNetCore.Mvc;

namespace MetroPorto.Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
[ServiceFilter(typeof(TokenAuthFilter))]
public class DataController : ControllerBase
{
    private readonly IGtfsDataService _gtfsDataService;
    private readonly ILogger<DataController> _logger;

    public DataController(IGtfsDataService gtfsDataService, ILogger<DataController> logger)
    {
        _gtfsDataService = gtfsDataService;
        _logger = logger;
    }

    [HttpPost("reload-data")]
    public async Task<IActionResult> ReloadData()
    {
        try
        {
            _logger.LogInformation("Iniciando carregamento manual de dados");
            await _gtfsDataService.LoadDataFromFilesAsync();
            return Ok(new { message = "Dados carregados com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recarregar dados");
            return StatusCode(500, new { message = "Erro ao carregar dados", error = ex.Message });
        }
    }
}