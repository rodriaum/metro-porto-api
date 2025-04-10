namespace Api.Models;

public class Trip
{
    public string Id { get; set; }
    public string RouteId { get; set; }
    public string ServiceId { get; set; }
    public string TripId { get; set; }
    public string TripHeadsign { get; set; }
    public int? DirectionId { get; set; }
    public string BlockId { get; set; }
    public string ShapeId { get; set; }
}