namespace MetroPorto.Api.Interfaces;

public interface IGtfsFileService
{
    Task<string> EnsureGtfsFilesExistAsync();
}