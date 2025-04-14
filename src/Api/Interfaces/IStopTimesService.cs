using MetroPorto.Api.Models;

namespace MetroPorto.Api.Interfaces;

public interface IStopTimesService
{
    Task<List<StopTime>> GetAllAsync();
    Task<List<StopTime>> GetByTripIdAsync(string tripId);
    Task<List<StopTime>> GetByStopIdAsync(string stopId);
    Task ImportDataAsync(string directoryPath);
}