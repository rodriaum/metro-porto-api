using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using MetroPorto.Api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetroPorto.Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
public class MetroController : ControllerBase
{
    private readonly IRoutesService _routesService;
    private readonly ITripsService _tripsService;
    private readonly IStopTimesService _stopTimesService;
    private readonly IStopsService _stopsService;
    private readonly ILogger<MetroController> _logger;

    public MetroController(
        IRoutesService routesService,
        ITripsService tripsService,
        IStopTimesService stopTimesService,
        IStopsService stopsService,
        ILogger<MetroController> logger)
    {
        _routesService = routesService;
        _tripsService = tripsService;
        _stopTimesService = stopTimesService;
        _stopsService = stopsService;
        _logger = logger;
    }

    [HttpGet("route-with-trips/{routeId}")]
    public async Task<ActionResult<RouteWithTripsDto>> GetRouteWithTrips(string routeId)
    {
        try
        {
            var route = await _routesService.GetByIdAsync(routeId);
            if (route == null)
                return NotFound(new { message = $"Rota com ID {routeId} não encontrada" });

            var trips = await _tripsService.GetByRouteIdAsync(routeId);

            return new RouteWithTripsDto
            {
                Route = route,
                Trips = trips
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar rota com viagens: {routeId}");
            return StatusCode(500, new { message = "Erro ao processar requisição", error = ex.Message });
        }
    }

    [HttpGet("trip-with-stops/{tripId}")]
    public async Task<ActionResult<TripWithStopTimesDto>> GetTripWithStops(string tripId)
    {
        try
        {
            var trip = await _tripsService.GetByIdAsync(tripId);
            if (trip == null)
                return NotFound(new { message = $"Viagem com ID {tripId} não encontrada" });

            var stopTimes = await _stopTimesService.GetByTripIdAsync(tripId);
            var stopTimesWithStops = new List<StopTimeWithStopDto>();

            foreach (var stopTime in stopTimes.OrderBy(st => st.StopSequence))
            {
                var stop = await _stopsService.GetByIdAsync(stopTime.StopId);
                stopTimesWithStops.Add(new StopTimeWithStopDto
                {
                    StopTime = stopTime,
                    Stop = stop
                });
            }

            return new TripWithStopTimesDto
            {
                Trip = trip,
                StopTimes = stopTimesWithStops
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar viagem com paradas: {tripId}");
            return StatusCode(500, new { message = "Erro ao processar requisição", error = ex.Message });
        }
    }

    [HttpGet("upcoming-departures/{stopId}")]
    public async Task<ActionResult<List<StopTimeWithStopDto>>> GetUpcomingDepartures(string stopId, [FromQuery] DateTime? referenceTime = null)
    {
        try
        {
            var stop = await _stopsService.GetByIdAsync(stopId);
            if (stop == null)
                return NotFound(new { message = $"Parada com ID {stopId} não encontrada" });

            // Se não for fornecido um horário de referência, usa o horário atual
            var reference = referenceTime ?? DateTime.Now;

            // Converte o horário para um formato comparável com os horários do GTFS (HH:mm:ss)
            var referenceTimeString = reference.ToString("HH:mm:ss");

            // Obtém todas as paradas para o stopId
            var stopTimes = await _stopTimesService.GetByStopIdAsync(stopId);

            // Filtra as próximas partidas e ordena por horário
            var upcomingDepartures = stopTimes
                .Where(st => string.Compare(st.DepartureTime, referenceTimeString) > 0)
                .OrderBy(st => st.DepartureTime)
                .Take(10) // Limite de 10 resultados
                .ToList();

            var result = new List<StopTimeWithStopDto>();
            foreach (var departure in upcomingDepartures)
            {
                var trip = await _tripsService.GetByIdAsync(departure.TripId);
                var route = trip != null ? await _routesService.GetByIdAsync(trip.RouteId) : null;

                result.Add(new StopTimeWithStopDto
                {
                    StopTime = departure,
                    Stop = stop
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar próximas partidas para parada: {stopId}");
            return StatusCode(500, new { message = "Erro ao processar requisição", error = ex.Message });
        }
    }
}