using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/porto/metro")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet("calendars")]
    public async Task<ActionResult<List<Calendar>>> GetAll()
    {
        return await _calendarService.GetAllAsync();
    }

    [HttpGet("calendars/{id}")]
    public async Task<ActionResult<Calendar>> GetById(string id)
    {
        var calendar = await _calendarService.GetByIdAsync(id);
        if (calendar == null)
            return NotFound();

        return calendar;
    }
}