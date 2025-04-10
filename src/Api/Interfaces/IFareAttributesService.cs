using Api.Models;

namespace Api.Interfaces;

public interface IFareAttributesService
{
    Task<List<FareAttribute>> GetAllAsync();
    Task<FareAttribute> GetByIdAsync(string fareId);
    Task ImportDataAsync(string directoryPath);
}