namespace MetroPorto.Api.Interfaces.Gtfs;

public interface IGtfsDataService
{
    Task InitializeAsync();
    Task LoadDataFromFilesAsync();
}