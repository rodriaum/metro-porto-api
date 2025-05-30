namespace MetroPortoAPI.Api;

public class Constant
{
    public const string GtfsFileUrl = "https://www.metrodoporto.pt/metrodoporto/uploads/document/file/693/google_transit_v2.zip";
    public const string ExtractPath = "./GtfsData";
    public const string ZipFilePath = "./google_transit_v2.zip";

    public const string Name = "Metro Porto";
    public const string Version = "1.0.0";

    public static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
}