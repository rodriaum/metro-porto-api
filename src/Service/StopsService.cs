using MongoDB.Driver;
using System.Globalization;
using TransitGtfsApi.Enums;
using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Models;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;

namespace TransitGtfsApi.Service;

public class StopsService : MongoService<Stop>, IStopsService
{
    private readonly IRedisService _redis;

    public StopsService(IMongoDatabase database, ILogger<StopsService> logger, IRedisService redis)
        : base(database, logger, "gtfs_stops")
    {
        _redis = redis;

        IndexKeysDefinition<Stop> indexKeysDefinition = Builders<Stop>.IndexKeys.Ascending(s => s.StopId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Stop>(indexKeysDefinition));
    }

    public async Task<List<Stop>> GetAllAsync()
    {
        return await _collection.Find(Builders<Stop>.Filter.Empty).ToListAsync();
    }

    public async Task<Stop?> GetByIdAsync(string stopId)
    {
        return await _redis.GetOrSetAsync(
            $"stop-{stopId}",
            async () => await _collection.Find(s => s.StopId == stopId).FirstOrDefaultAsync()
        );
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "stops.txt");
        await ImportFromCsvAsync(filePath, fields => new Stop
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            StopId = fields.GetValueOrDefault("stop_id", "") ?? "",
            StopCode = fields.GetValueOrDefault("stop_code", "") ?? "",
            StopName = fields.GetValueOrDefault("stop_name", "") ?? "",
            StopDesc = fields.GetValueOrDefault("stop_desc", "") ?? "",
            StopLat = NumberUtil.ParseDoubleSafe(fields.GetValueOrDefault("stop_lat", null), format: CultureInfo.InvariantCulture),
            StopLon = NumberUtil.ParseDoubleSafe(fields.GetValueOrDefault("stop_lon", null), format: CultureInfo.InvariantCulture),
            ZoneId = fields.GetValueOrDefault("zone_id", "") ?? "",
            StopUrl = fields.GetValueOrDefault("stop_url", "") ?? "",
            LocationType = EnumUtil.FromValue<LocationType>(NumberUtil.ParseIntSafe(fields.GetValueOrDefault("location_type", null))),
            ParentStation = fields.GetValueOrDefault("parent_station", "") ?? "",
            StopTimezone = fields.GetValueOrDefault("stop_timezone", "") ?? "",
            WheelchairBoarding = EnumUtil.FromValue<AccessibilityType>(NumberUtil.ParseIntSafe(fields.GetValueOrDefault("wheelchair_boarding", null))),
            PlatformCode = fields.GetValueOrDefault("platform_code", "") ?? ""
        });
    }
}