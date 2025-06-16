namespace MetroPortoAPI.Api;

public class Constant
{
    public static readonly List<string> GtfsFileUrls = new List<string>
    {
        // Metro Porto
        "https://www.metrodoporto.pt/metrodoporto/uploads/document/file/693/google_transit_v2.zip",
        // STCP
        "https://opendata.porto.digital/dataset/5275c986-592c-43f5-8f87-aabbd4e4f3a4/resource/89a6854f-2ea3-4ba0-8d2f-6558a9df2a98/download/horarios_gtfs_stcp_16_04_2025.zip"
    };
    public const string ExtractPath = "./GtfsData";
    public const string TempDownloadFolder = "./TempGtfs";

    public const string Name = "Transit GTFS";
    public const string Version = "1.0.0";

    public static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public static readonly List<string> GtfsCollections = new List<string>
    {
        "gtfs_agency",
        "gtfs_calendar",
        "gtfs_calendar_dates",
        "gtfs_fare_attributes",
        "gtfs_fare_rules",
        "gtfs_routes",
        "gtfs_shapes",
        "gtfs_stops",
        "gtfs_stop_times",
        "gtfs_transfers",
        "gtfs_trips"
    };

    public static readonly string[] RequiredEnvVars = new[]
{
        "MONGODB_CONNECTION",
        "REDIS_CONNECTION",
        "MONGODB_DATABASE_NAME",
        "REDIS_INSTANCE_NAME"
    };
}