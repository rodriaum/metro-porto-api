namespace TransitGtfs.Api.Models;
public class Route
{
    public string Id { get; set; }
    public string RouteId { get; set; }
    public string AgencyId { get; set; }
    public string RouteShortName { get; set; }
    public string RouteLongName { get; set; }
    public string RouteDesc { get; set; }
    public int? RouteType { get; set; } // 0=Tram/Light rail, 1=Subway/Metro, 2=Rail, 3=Bus, 4=Ferry, 5=Cable car, 6=Gondola/Suspended cable car, 7=Funicular
    public string RouteUrl { get; set; }
    public string RouteColor { get; set; }
    public string RouteTextColor { get; set; }
}