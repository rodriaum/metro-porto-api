using TransitGtfs.Api.Interfaces;
using TransitGtfs.Api.Interfaces.Database;
using TransitGtfs.Api.Service.Database;
using TransitGtfs.Api.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TransitGtfs.Api.Service;

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

        await ImportFromCsvAsync(filePath, fields => new Models.Route
        {
            Id = ObjectId.GenerateNewId().ToString(),
            RouteId = fields.GetValueOrDefault("route_id", "") ?? "",
            AgencyId = fields.GetValueOrDefault("agency_id", "") ?? "",
            RouteShortName = fields.GetValueOrDefault("route_short_name", "") ?? "",
            RouteLongName = fields.GetValueOrDefault("route_long_name", "") ?? "",
            RouteDesc = fields.GetValueOrDefault("route_desc", "") ?? "",
            RouteType = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("route_type", null)),
            RouteUrl = fields.GetValueOrDefault("route_url", "") ?? "",
            RouteColor = fields.GetValueOrDefault("route_color", "") ?? "",
            RouteTextColor = fields.GetValueOrDefault("route_text_color", "") ?? "",
        });
    }
}