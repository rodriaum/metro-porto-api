using MetroPortoAPI.Api.Models;

namespace MetroPortoAPI.Api.Models.DTOs;

public class RouteWithTripsDto
{
    public Route Route { get; set; }
    public List<Trip> Trips { get; set; }
}