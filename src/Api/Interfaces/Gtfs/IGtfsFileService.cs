namespace MetroPortoAPI.Api.Interfaces.Gtfs;

public interface IGtfsFileService
{
    Task<string> EnsureGtfsFilesExistAsync();
}