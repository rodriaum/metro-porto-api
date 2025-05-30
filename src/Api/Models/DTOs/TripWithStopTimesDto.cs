namespace MetroPortoAPI.Api.Models.DTOs;

public class TripWithStopTimesDto
{
    public Trip Trip { get; set; }
    public List<StopTimeWithStopDto> StopTimes { get; set; }
}