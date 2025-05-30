using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

public class StopsService : MongoService<Stop>, IStopsService
{
    private readonly IRedisService _redis;

    public StopsService(IMongoDatabase database, ILogger<StopsService> logger, IRedisService redis)
        : base(database, logger, "stops")
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
            StopId = fields[0],
            StopCode = fields.Length > 1 ? fields[1] : "",
            StopName = fields.Length > 2 ? fields[2] : "",
            StopDesc = fields.Length > 3 ? fields[3] : "",
            StopLat = double.Parse(fields[4], System.Globalization.CultureInfo.InvariantCulture),
            StopLon = double.Parse(fields[5], System.Globalization.CultureInfo.InvariantCulture),
            ZoneId = fields.Length > 6 ? fields[6] : "",
            StopUrl = fields.Length > 7 ? fields[7] : ""
        });
    }
}