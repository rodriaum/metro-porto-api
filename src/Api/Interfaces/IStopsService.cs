using MetroPorto.Api.Models;

namespace MetroPorto.Api.Interfaces;

public interface IStopsService
{
    Task<List<Stop>> GetAllAsync();
    Task<Stop> GetByIdAsync(string stopId);
    Task ImportDataAsync(string directoryPath);
}