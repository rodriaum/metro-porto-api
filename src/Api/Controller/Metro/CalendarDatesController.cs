using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.Metro;

[ApiController]
[Route("v1/porto/metro")]
public class CalendarDatesController : ControllerBase
{
    private readonly ICalendarDatesService _calendarDatesService;

    public CalendarDatesController(ICalendarDatesService calendarDatesService)
    {
        _calendarDatesService = calendarDatesService;
    }

    [HttpGet("calendar-dates")]
    public async Task<ActionResult<List<CalendarDate>>> GetAll()
    {
        return await _calendarDatesService.GetAllAsync();
    }

    [HttpGet("calendar-dates/service/{serviceId}")]
    public async Task<ActionResult<List<CalendarDate>>> GetByServiceId(string serviceId)
    {
        return await _calendarDatesService.GetByServiceIdAsync(serviceId);
    }
}