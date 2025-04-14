namespace MetroPorto.Api.Models;
public class Route
{
    public string Id { get; set; }
    public string RouteId { get; set; }
    public string AgencyId { get; set; }
    public string RouteShortName { get; set; }
    public string RouteLongName { get; set; }
    public string RouteDesc { get; set; }
    public int RouteType { get; set; }
    public string RouteUrl { get; set; }
    public string RouteColor { get; set; }
    public string RouteTextColor { get; set; }
}