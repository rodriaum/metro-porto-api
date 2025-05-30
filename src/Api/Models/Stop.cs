namespace MetroPortoAPI.Api.Models;

public class Stop
{
    public string Id { get; set; }
    public string StopId { get; set; }
    public string StopCode { get; set; }
    public string StopName { get; set; }
    public string StopDesc { get; set; }
    public double StopLat { get; set; }
    public double StopLon { get; set; }
    public string ZoneId { get; set; }
    public string StopUrl { get; set; }
}