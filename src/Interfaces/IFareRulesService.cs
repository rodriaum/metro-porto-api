using TransitGtfsApi.Models;

namespace TransitGtfsApi.Interfaces;

public interface IFareRulesService
{
    Task<List<FareRule>> GetAllAsync();
    Task<List<FareRule>?> GetByFareIdAsync(string fareId);
    Task ImportDataAsync(string directoryPath);
}