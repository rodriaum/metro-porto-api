using Api.Models;

namespace Api.Interfaces;

public interface ICalendarDatesService
{
    Task<List<CalendarDate>> GetAllAsync();
    Task<List<CalendarDate>> GetByServiceIdAsync(string serviceId);
    Task ImportDataAsync(string directoryPath);
}