using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Gtfs;
using TransitGtfsApi.Models;
using MongoDB.Driver;

namespace TransitGtfsApi.Service.Gtfs;

public class GtfsDataService : IGtfsDataService
{
    private readonly IGtfsFileService _gtfsFileService;
    private readonly IAgencyService _agencyService;
    private readonly ICalendarService _calendarService;
    private readonly ICalendarDatesService _calendarDatesService;
    private readonly IFareAttributesService _fareAttributesService;
    private readonly IFareRulesService _fareRulesService;
    private readonly IRoutesService _routesService;
    private readonly IShapesService _shapesService;
    private readonly IStopsService _stopsService;
    private readonly IStopTimesService _stopTimesService;
    private readonly ITransfersService _transfersService;
    private readonly ITripsService _tripsService;
    private readonly ILogger<GtfsDataService> _logger;
    private readonly IMongoDatabase _database;

    public GtfsDataService(
        IGtfsFileService gtfsFileService,
        IAgencyService agencyService,
        ICalendarService calendarService,
        ICalendarDatesService calendarDatesService,
        IFareAttributesService fareAttributesService,
        IFareRulesService fareRulesService,
        IRoutesService routesService,
        IShapesService shapesService,
        IStopsService stopsService,
        IStopTimesService stopTimesService,
        ITransfersService transfersService,
        ITripsService tripsService,
        ILogger<GtfsDataService> logger,
        IMongoDatabase database)
    {
        _gtfsFileService = gtfsFileService;
        _agencyService = agencyService;
        _calendarService = calendarService;
        _calendarDatesService = calendarDatesService;
        _fareAttributesService = fareAttributesService;
        _fareRulesService = fareRulesService;
        _routesService = routesService;
        _shapesService = shapesService;
        _stopsService = stopsService;
        _stopTimesService = stopTimesService;
        _transfersService = transfersService;
        _tripsService = tripsService;
        _logger = logger;
        _database = database;
    }

    public async Task InitializeAsync()
    {
        try
        {
            IMongoCollection<Agency> agencyCollection = _database.GetCollection<Agency>("agency");
            bool dataExists = await agencyCollection.CountDocumentsAsync(FilterDefinition<Agency>.Empty) > 0;

            if (!dataExists)
            {
                _logger.LogInformation("No data found in MongoDB. Loading data from GTFS files.");
                await LoadDataFromFilesAsync();
            }
            else
            {
                _logger.LogInformation("Data already exists in MongoDB.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing GTFS data service");
            throw;
        }
    }

    public async Task LoadDataFromFilesAsync()
    {
        try
        {
            List<string> gtfsDirectories = await _gtfsFileService.EnsureGtfsFilesExistAsync();

            if (gtfsDirectories.Count == 0)
            {
                _logger.LogWarning("No GTFS directory found for import.");
                return;
            }

            _logger.LogInformation($"Starting import of data from {gtfsDirectories.Count} GTFS sources");

            foreach (string collectionName in Constant.GtfsCollections)
            {
                _logger.LogInformation($"Clearing collection {collectionName}");
                await _database.DropCollectionAsync(collectionName);
            }

            foreach (string gtfsDirectoryPath in gtfsDirectories)
            {
                _logger.LogInformation($"Importing GTFS data from {gtfsDirectoryPath}");

                List<Task> tasks = new List<Task>
                {
                    _agencyService.ImportDataAsync(gtfsDirectoryPath),
                    _calendarService.ImportDataAsync(gtfsDirectoryPath),
                    _calendarDatesService.ImportDataAsync(gtfsDirectoryPath),
                    _fareAttributesService.ImportDataAsync(gtfsDirectoryPath),
                    _fareRulesService.ImportDataAsync(gtfsDirectoryPath),
                    _routesService.ImportDataAsync(gtfsDirectoryPath),
                    _shapesService.ImportDataAsync(gtfsDirectoryPath),
                    _stopsService.ImportDataAsync(gtfsDirectoryPath),
                    _stopTimesService.ImportDataAsync(gtfsDirectoryPath),
                    _transfersService.ImportDataAsync(gtfsDirectoryPath),
                    _tripsService.ImportDataAsync(gtfsDirectoryPath)
                };

                await Task.WhenAll(tasks);
                _logger.LogInformation($"Data import from {gtfsDirectoryPath} completed successfully");
            }

            _logger.LogInformation("Import of all GTFS data completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data from GTFS files");
            throw;
        }
    }
}