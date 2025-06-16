using TransitGtfsApi.Models;

namespace TransitGtfsApi.Models.DTOs;

public class RouteWithTripsDto
{
    public Route Route { get; set; }
    public List<Trip> Trips { get; set; }
}