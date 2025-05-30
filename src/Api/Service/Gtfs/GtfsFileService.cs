using System.IO.Compression;
using MetroPortoAPI.Api.Interfaces.Gtfs;
using MetroPortoAPI.Api;

namespace MetroPortoAPI.Api.Service.Gtfs;

public class GtfsFileService : IGtfsFileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GtfsFileService> _logger;

    public GtfsFileService(IHttpClientFactory httpClientFactory, ILogger<GtfsFileService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> EnsureGtfsFilesExistAsync()
    {
        // Check if directory exists
        if (!Directory.Exists(Constant.ExtractPath))
        {
            _logger.LogInformation("GTFS data directory doesn't exist. Creating and downloading data.");
            Directory.CreateDirectory(Constant.ExtractPath);

            await DownloadGtfsFileAsync();
            ExtractGtfsFile();
        }
        else
        {
            // Check if files exist in directory
            string[] requiredFiles = { "agency.txt", "calendar.txt", "calendar_dates.txt", "fare_attributes.txt",
                                        "fare_rules.txt", "routes.txt", "shapes.txt", "stops.txt",
                                        "stop_times.txt", "transfers.txt", "trips.txt" };

            bool allFilesExist = true;
            foreach (var file in requiredFiles)
            {
                if (!File.Exists(Path.Combine(Constant.ExtractPath, file)))
                {
                    allFilesExist = false;
                    break;
                }
            }

            if (!allFilesExist)
            {
                _logger.LogInformation("Some GTFS data files are missing. Downloading again.");
                await DownloadGtfsFileAsync();
                ExtractGtfsFile();
            }
            else
            {
                _logger.LogInformation("GTFS data files already exist.");
            }
        }

        return Constant.ExtractPath;
    }

    private async Task DownloadGtfsFileAsync()
    {
        _logger.LogInformation($"Downloading GTFS data from {Constant.GtfsFileUrl}");
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(Constant.GtfsFileUrl);
        response.EnsureSuccessStatusCode();

        using (var fileStream = new FileStream(Constant.ZipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await response.Content.CopyToAsync(fileStream);
        }
        _logger.LogInformation("Download completed successfully.");
    }

    private void ExtractGtfsFile()
    {
        _logger.LogInformation($"Extracting GTFS data to {Constant.ExtractPath}");

        // Clear the directory before extraction
        if (Directory.Exists(Constant.ExtractPath))
        {
            var di = new DirectoryInfo(Constant.ExtractPath);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
        }

        ZipFile.ExtractToDirectory(Constant.ZipFilePath, Constant.ExtractPath, overwriteFiles: true);
        _logger.LogInformation("Extraction completed successfully.");
    }
}