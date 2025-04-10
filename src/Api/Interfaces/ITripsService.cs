using Api.Models;

namespace Api.Interfaces;

public interface ITripsService
{
    Task<List<Trip>> GetAllAsync();
    Task<Trip> GetByIdAsync(string tripId);
    Task<List<Trip>> GetByRouteIdAsync(string routeId);
    Task ImportDataAsync(string directoryPath);
}