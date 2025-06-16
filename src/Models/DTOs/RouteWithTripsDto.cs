using TransitGtfs.Api.Models;

namespace TransitGtfs.Api.Models.DTOs;

public class RouteWithTripsDto
{
    public Route Route { get; set; }
    public List<Trip> Trips { get; set; }
}