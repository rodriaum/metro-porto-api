using Api.Models;

namespace Api.Interfaces;

public interface ICalendarService
{
    Task<List<Calendar>> GetAllAsync();
    Task<Calendar> GetByIdAsync(string serviceId);
    Task ImportDataAsync(string directoryPath);
}