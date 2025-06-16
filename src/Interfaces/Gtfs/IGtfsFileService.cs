namespace TransitGtfs.Api.Interfaces.Gtfs;

public interface IGtfsFileService
{
    Task<List<string>> EnsureGtfsFilesExistAsync();
}