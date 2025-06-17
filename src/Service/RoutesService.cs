using MongoDB.Bson;
using MongoDB.Driver;
using TransitGtfsApi.Enums;
using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;

namespace TransitGtfsApi.Service;

public class RoutesService : MongoService<Models.Route>, IRoutesService
{
    private readonly IRedisService _redis;

    public RoutesService(IMongoDatabase database, ILogger<RoutesService> logger, IRedisService redis)
        : base(database, logger, "gtfs_routes")
    {
        _redis = redis;

        IndexKeysDefinition<Models.Route> indexKeysDefinition = Builders<Models.Route>.IndexKeys.Ascending(r => r.RouteId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Models.Route>(indexKeysDefinition));
    }

    public async Task<List<Models.Route>> GetAllAsync()
    {
        return await _collection.Find(Builders<Models.Route>.Filter.Empty).ToListAsync();
    }

    public async Task<Models.Route?> GetByIdAsync(string routeId)
    {
        return await _redis.GetOrSetAsync(
            $"route-{routeId}",
            async () => await _collection.Find(r => r.RouteId == routeId).FirstOrDefaultAsync()
        );
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "routes.txt");

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {FilePath}", filePath);
            return;
        }

        await ImportFromCsvAsync(filePath, fields =>
        {
            int routeTypeId = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("route_type", null), -1);

            if (!EnumUtil.TryFromValue(routeTypeId, out RouteType routeType))
            {
                _logger.LogWarning("Cannot parse route_type value: {routeTypeId}", routeTypeId);
                return null;
            }

            return new Models.Route
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RouteId = fields.GetValueOrDefault("route_id", "") ?? "",
                AgencyId = fields.GetValueOrDefault("agency_id", "") ?? "",
                RouteShortName = fields.GetValueOrDefault("route_short_name", "") ?? "",
                RouteLongName = fields.GetValueOrDefault("route_long_name", "") ?? "",
                RouteDesc = fields.GetValueOrDefault("route_desc", "") ?? "",
                RouteType = routeType,
                RouteUrl = fields.GetValueOrDefault("route_url", "") ?? "",
                RouteColor = fields.GetValueOrDefault("route_color", "") ?? "",
                RouteTextColor = fields.GetValueOrDefault("route_text_color", "") ?? "",
                RouteSortOrder = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("route_sort_order", null)),
                ContinuousPickup = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("continuous_pickup", null)),
                ContinuousDropOff = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("continuous_drop_off", null))
            };
        });
    }
}