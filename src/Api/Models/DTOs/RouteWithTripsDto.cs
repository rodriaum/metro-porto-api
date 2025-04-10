namespace Api.Models;

public class RouteWithTripsDto
{
    public Route Route { get; set; }
    public List<Trip> Trips { get; set; }
}