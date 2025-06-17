using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Models;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;
using MongoDB.Driver;
using System.Globalization;
using TransitGtfsApi.Enums;

namespace TransitGtfsApi.Service;

public class StopTimesService : MongoService<StopTime>, IStopTimesService
{
    private readonly IRedisService _redis;

    public StopTimesService(IMongoDatabase database, ILogger<StopTimesService> logger, IRedisService redis)
        : base(database, logger, "gtfs_stop_times")
    {
        _redis = redis;

        IndexKeysDefinition<StopTime> indexKeysDefinition = Builders<StopTime>.IndexKeys.Ascending(st => st.TripId);
        _collection.Indexes.CreateOne(new CreateIndexModel<StopTime>(indexKeysDefinition));

        indexKeysDefinition = Builders<StopTime>.IndexKeys.Ascending(st => st.StopId);
        _collection.Indexes.CreateOne(new CreateIndexModel<StopTime>(indexKeysDefinition));
    }

    public async Task<List<StopTime>> GetAllAsync(int page = 1, int pageSize = 100)
    {
        int skip = (page - 1) * pageSize;

        return await _collection.Find(_ => true)
                                .Skip(skip)
                                .Limit(pageSize)
                                .ToListAsync();
    }

    public async Task<List<StopTime>?> GetByTripIdAsync(string tripId)
    {
        return await _redis.GetOrSetAsync(
            $"stop-times-trip-{tripId}",
            async () => await _collection.Find(st => st.TripId == tripId).ToListAsync()
        );
    }

    public async Task<List<StopTime>> GetByStopIdAsync(string stopId, int page = 1, int pageSize = 100)
    {
        return await _redis.GetOrSetAsync(
            $"stop-times-stop-{stopId}-{page}-{pageSize}",
            async () =>
            {
                var skip = (page - 1) * pageSize;
                return await _collection.Find(st => st.StopId == stopId)
                                        .Skip(skip)
                                        .Limit(pageSize)
                                        .ToListAsync();
            }
        ) ?? new List<StopTime>();
    }

public async Task ImportDataAsync(string directoryPath)
{
    string filePath = Path.Combine(directoryPath, "stop_times.txt");
    
    if (!File.Exists(filePath))
    {
        _logger.LogWarning("File not found: {FilePath}", filePath);
        return;
    }
    
    await ImportFromCsvAsync(filePath, fields =>
    {
        int pickupTypeId = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("pickup_type", null), -1);
        int dropOffTypeId = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("drop_off_type", null), -1);
        int timepointId = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("timepoint", null), -1);
        
        return new StopTime
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            TripId = fields.GetValueOrDefault("trip_id", "") ?? "",
            ArrivalTime = fields.GetValueOrDefault("arrival_time", "") ?? "",
            DepartureTime = fields.GetValueOrDefault("departure_time", "") ?? "",
            StopId = fields.GetValueOrDefault("stop_id", "") ?? "",
            StopSequence = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("stop_sequence", null)),
            StopHeadsign = fields.GetValueOrDefault("stop_headsign", "") ?? "",
            PickupType = pickupTypeId != -1 ? EnumUtil.FromValue<PickupType>(pickupTypeId) : null,
            DropOffType = dropOffTypeId,
            ShapeDistTraveled = NumberUtil.ParseDoubleSafe(fields.GetValueOrDefault("shape_dist_traveled", null), format: CultureInfo.InvariantCulture),
            Timepoint = timepointId != -1 ? EnumUtil.FromValue<TimepointType>(timepointId) : null
        };
    });
}
}