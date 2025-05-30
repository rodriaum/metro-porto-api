using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Gtfs;
using MetroPortoAPI.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service.Gtfs;

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
            // Check if any data exists in MongoDB
            var agencyCollection = _database.GetCollection<Agency>("agency");
            var dataExists = await agencyCollection.CountDocumentsAsync(FilterDefinition<Agency>.Empty) > 0;

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
            string gtfsDirectoryPath = await _gtfsFileService.EnsureGtfsFilesExistAsync();

            _logger.LogInformation("Starting data import from GTFS files");

            // Import data in parallel
            var tasks = new List<Task>
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
            _logger.LogInformation("Data import completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data from GTFS files");
            throw;
        }
    }
}