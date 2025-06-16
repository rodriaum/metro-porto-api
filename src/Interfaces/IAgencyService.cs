using TransitGtfs.Api.Models;

namespace TransitGtfs.Api.Interfaces;

public interface IAgencyService
{
    Task<List<Agency>> GetAllAsync();
    Task<Agency?> GetByIdAsync(string agencyId);
    Task ImportDataAsync(string directoryPath);
}