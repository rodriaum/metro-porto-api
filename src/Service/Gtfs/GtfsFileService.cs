using TransitGtfsApi.Interfaces.Gtfs;
using System.IO.Compression;

namespace TransitGtfsApi.Service.Gtfs;

public class GtfsFileService : IGtfsFileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GtfsFileService> _logger;

    public GtfsFileService(IHttpClientFactory httpClientFactory, ILogger<GtfsFileService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<string>> EnsureGtfsFilesExistAsync()
    {
        List<string> gtfsDirectories = new List<string>();

        if (!Directory.Exists(Constant.ExtractPath))
        {
            Directory.CreateDirectory(Constant.ExtractPath);
        }

        if (Constant.GtfsFileUrls.Count == 0)
        {
            _logger.LogWarning("No GTFS URLs configured. Please configure at least one URL in Constant.GtfsFileUrls.");
            return gtfsDirectories;
        }

        for (int i = 0; i < Constant.GtfsFileUrls.Count; i++)
        {
            string gtfsUrl = Constant.GtfsFileUrls[i];
            string providerFolderName = $"provider_{i + 1}";
            string providerDirectory = Path.Combine(Constant.ExtractPath, providerFolderName);

            if (!Directory.Exists(providerDirectory))
            {
                Directory.CreateDirectory(providerDirectory);
            }

            bool needsDownload = !AreRequiredFilesPresent(providerDirectory);

            if (needsDownload)
            {
                _logger.LogInformation($"Downloading and extracting GTFS data from {gtfsUrl} to {providerDirectory}");
                string tempZipPath = Path.Combine(Constant.TempDownloadFolder, $"gtfs_{i + 1}.zip");

                if (!Directory.Exists(Constant.TempDownloadFolder))
                {
                    Directory.CreateDirectory(Constant.TempDownloadFolder);
                }

                await DownloadGtfsFileAsync(gtfsUrl, tempZipPath);
                ExtractGtfsFile(tempZipPath, providerDirectory);

                try
                {
                    if (File.Exists(tempZipPath))
                    {
                        File.Delete(tempZipPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Could not delete temporary file {tempZipPath}");
                }
            }
            else
            {
                _logger.LogInformation($"GTFS files for {providerFolderName} already exist at {providerDirectory}");
            }

            gtfsDirectories.Add(providerDirectory);
        }

        try
        {
            if (Directory.Exists(Constant.TempDownloadFolder))
            {
                Directory.Delete(Constant.TempDownloadFolder, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete temporary directory. Some temporary files may remain.");
        }

        return gtfsDirectories;
    }

    private bool AreRequiredFilesPresent(string directoryPath)
    {
        string[] requiredFiles = { "agency.txt", "calendar.txt", "calendar_dates.txt", "fare_attributes.txt",
                                   "fare_rules.txt", "routes.txt", "shapes.txt", "stops.txt",
                                   "stop_times.txt", "transfers.txt", "trips.txt" };

        foreach (string file in requiredFiles)
        {
            if (!File.Exists(Path.Combine(directoryPath, file)))
            {
                return false;
            }
        }

        return true;
    }

    private async Task DownloadGtfsFileAsync(string url, string filePath)
    {
        _logger.LogInformation($"Downloading GTFS data from {url}");

        HttpClient httpClient = _httpClientFactory.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await response.Content.CopyToAsync(fileStream);
        }

        _logger.LogInformation($"Download completed successfully for {url}");
    }

    private void ExtractGtfsFile(string zipFilePath, string extractPath)
    {
        _logger.LogInformation($"Extracting GTFS data from {zipFilePath} to {extractPath}");

        if (Directory.Exists(extractPath))
        {
            DirectoryInfo di = new DirectoryInfo(extractPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);
        _logger.LogInformation("Extraction completed successfully.");
    }
}
