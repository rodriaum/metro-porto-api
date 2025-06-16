using TransitGtfs.Api.Models;

namespace TransitGtfs.Api.Interfaces;

public interface ICalendarDatesService
{
    Task<List<CalendarDate>> GetAllAsync();
    Task<List<CalendarDate>?> GetByServiceIdAsync(string serviceId);
    Task ImportDataAsync(string directoryPath);
}