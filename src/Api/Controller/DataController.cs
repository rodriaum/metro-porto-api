using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
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