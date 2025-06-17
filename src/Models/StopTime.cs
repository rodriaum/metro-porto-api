using TransitGtfsApi.Enums;

namespace TransitGtfsApi.Models;
public class StopTime
{
    public string Id { get; set; }
    public string TripId { get; set; }
    public string ArrivalTime { get; set; }
    public string DepartureTime { get; set; }
    public string StopId { get; set; }
    public int StopSequence { get; set; }
    public string? StopHeadsign { get; set; }
    public PickupType? PickupType { get; set; }
    public int? DropOffType { get; set; }
    public double? ShapeDistTraveled { get; set; }
    public TimepointType? Timepoint { get; set; }
}