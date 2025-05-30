using MetroPortoAPI.Api.Models;

namespace MetroPortoAPI.Api.Interfaces;

public interface IStopsService
{
    Task<List<Stop>> GetAllAsync();
    Task<Stop?> GetByIdAsync(string stopId);
    Task ImportDataAsync(string directoryPath);
}