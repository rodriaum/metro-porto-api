using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Service.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

public class RoutesService : MongoService<Models.Route>, IRoutesService
{
    private readonly IRedisService _redis;

    public RoutesService(IMongoDatabase database, ILogger<RoutesService> logger, IRedisService redis)
        : base(database, logger, "routes")
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
            RouteId = fields[0],
            AgencyId = fields.Length > 1 ? fields[1] : "",
            RouteShortName = fields.Length > 2 ? fields[2] : "",
            RouteLongName = fields.Length > 3 ? fields[3] : "",
            RouteDesc = fields.Length > 4 ? fields[4] : "",
            RouteType = int.Parse(fields[5]),
            RouteUrl = fields.Length > 6 ? fields[6] : "",
            RouteColor = fields.Length > 7 ? fields[7] : "",
            RouteTextColor = fields.Length > 8 ? fields[8] : ""
        });
    }
}