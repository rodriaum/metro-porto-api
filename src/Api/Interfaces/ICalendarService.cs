using MetroPortoAPI.Api.Models;

namespace MetroPortoAPI.Api.Interfaces;

public interface ICalendarService
{
    Task<List<Calendar>> GetAllAsync();
    Task<Calendar?> GetByIdAsync(string serviceId);
    Task ImportDataAsync(string directoryPath);
}