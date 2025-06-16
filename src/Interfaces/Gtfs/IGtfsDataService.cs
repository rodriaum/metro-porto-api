namespace TransitGtfsApi.Interfaces.Gtfs;

public interface IGtfsDataService
{
    Task InitializeAsync();
    Task LoadDataFromFilesAsync();
}