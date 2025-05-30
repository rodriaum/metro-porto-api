namespace MetroPortoAPI.Api.Interfaces;

public interface IRoutesService
{
    Task<List<Models.Route>> GetAllAsync();
    Task<Models.Route?> GetByIdAsync(string routeId);
    Task ImportDataAsync(string directoryPath);
}