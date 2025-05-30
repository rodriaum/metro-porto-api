using MetroPortoAPI.Api.Models;

namespace MetroPortoAPI.Api.Interfaces;

public interface IAgencyService
{
    Task<List<Agency>> GetAllAsync();
    Task<Agency?> GetByIdAsync(string agencyId);
    Task ImportDataAsync(string directoryPath);
}