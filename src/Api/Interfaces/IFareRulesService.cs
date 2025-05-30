using MetroPortoAPI.Api.Models;

namespace MetroPortoAPI.Api.Interfaces;

public interface IFareRulesService
{
    Task<List<FareRule>> GetAllAsync();
    Task<List<FareRule>?> GetByFareIdAsync(string fareId);
    Task ImportDataAsync(string directoryPath);
}