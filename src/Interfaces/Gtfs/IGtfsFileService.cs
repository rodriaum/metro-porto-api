namespace TransitGtfsApi.Interfaces.Gtfs;

public interface IGtfsFileService
{
    Task<List<string>> EnsureGtfsFilesExistAsync();
}