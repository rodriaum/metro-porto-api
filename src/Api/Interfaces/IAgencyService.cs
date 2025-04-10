using Api.Models;

namespace Api.Interfaces;

public interface IAgencyService
{
    Task<List<Agency>> GetAllAsync();
    Task<Agency> GetByIdAsync(string agencyId);
    Task ImportDataAsync(string directoryPath);
}