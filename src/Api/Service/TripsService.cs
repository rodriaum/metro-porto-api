using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Interfaces.Database;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class TripsService : MongoService<Trip>, ITripsService
{
    private readonly IRedisService _redis;

    public TripsService(IMongoDatabase database, ILogger<TripsService> logger, IRedisService redis)
        : base(database, logger, "trips")
    {
        _redis = redis;

        IndexKeysDefinition<Trip> indexKeysDefinition = Builders<Trip>.IndexKeys.Ascending(t => t.TripId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Trip>(indexKeysDefinition));

        indexKeysDefinition = Builders<Trip>.IndexKeys.Ascending(t => t.RouteId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Trip>(indexKeysDefinition));

        indexKeysDefinition = Builders<Trip>.IndexKeys.Ascending(t => t.ServiceId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Trip>(indexKeysDefinition));
    }

    public async Task<List<Trip>> GetAllAsync(int page = 1, int pageSize = 100)
    {
        int skip = (page - 1) * pageSize;

        return await _collection.Find(Builders<Trip>.Filter.Empty)
                                .Skip(skip)
                                .Limit(pageSize)
                                .ToListAsync();
    }

    public async Task<Trip?> GetByIdAsync(string tripId)
    {
        return await _redis.GetOrSetAsync(
            $"trip-{tripId}",
            async () => await _collection.Find(t => t.TripId == tripId).FirstOrDefaultAsync()
        );
    }

    public async Task<List<Trip>?> GetByRouteIdAsync(string routeId, int page = 1, int pageSize = 100)
    {
        return await _redis.GetOrSetAsync(
            $"trips-route-{routeId}-{page}-{pageSize}",
            async () =>
            {
                var skip = (page - 1) * pageSize;
                return await _collection.Find(t => t.RouteId == routeId)
                                        .Skip(skip)
                                        .Limit(pageSize)
                                        .ToListAsync();
            }
        );
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "trips.txt");

        await ImportFromCsvAsync(filePath, fields => new Trip
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            RouteId = fields[0],
            ServiceId = fields[1],
            TripId = fields[2],
            TripHeadsign = fields.Length > 3 ? fields[3] : "",
            DirectionId = fields.Length > 4 && !string.IsNullOrEmpty(fields[4]) ? int.Parse(fields[4]) : (int?)null,
            BlockId = fields.Length > 5 ? fields[5] : "",
            ShapeId = fields.Length > 6 ? fields[6] : ""
        });
    }
}