namespace MetroPortoAPI.Api.Models;

public class Stop
{
    public string Id { get; set; }
    public string StopId { get; set; }
    public string StopCode { get; set; }
    public string StopName { get; set; }
    public string StopDesc { get; set; }
    public double? StopLat { get; set; }
    public double? StopLon { get; set; }
    public string ZoneId { get; set; }
    public string StopUrl { get; set; }
    public int? LocationType { get; set; } // 0=stop, 1=station
    public string? ParentStation { get; set; }
    public string? StopTimezone { get; set; }
    public int? WheelchairBoarding { get; set; } // 0=unknown, 1=accessible, 2=no
}