using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Interfaces.Database;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class StopTimesService : MongoService<StopTime>, IStopTimesService
{
    private readonly IRedisService _redis;

    public StopTimesService(IMongoDatabase database, ILogger<StopTimesService> logger, IRedisService redis)
        : base(database, logger, "stop_times")
    {
        _redis = redis;

        IndexKeysDefinition<StopTime> indexKeysDefinition = Builders<StopTime>.IndexKeys.Ascending(st => st.TripId);
        _collection.Indexes.CreateOne(new CreateIndexModel<StopTime>(indexKeysDefinition));

        indexKeysDefinition = Builders<StopTime>.IndexKeys.Ascending(st => st.StopId);
        _collection.Indexes.CreateOne(new CreateIndexModel<StopTime>(indexKeysDefinition));
    }

    public async Task<List<StopTime>> GetAllAsync()
    {
        return await _redis.GetOrSetAsync(
            "stop-times-all",
            async () => await _collection.Find(_ => true).ToListAsync()
        ) ?? new List<StopTime>();
    }

    public async Task<List<StopTime>> GetByTripIdAsync(string tripId)
    {
        return await _redis.GetOrSetAsync(
            $"stop-times-trip-{tripId}",
            async () => await _collection.Find(st => st.TripId == tripId).ToListAsync()
        ) ?? new List<StopTime>();
    }

    public async Task<List<StopTime>> GetByStopIdAsync(string stopId, int page = 1, int pageSize = 100)
    {
        return await _redis.GetOrSetAsync(
            $"stop-times-stop-{stopId}-{page}-{pageSize}",
            async () => {
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
        await ImportFromCsvAsync(filePath, fields => new StopTime
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            TripId = fields[0],
            ArrivalTime = fields[1],
            DepartureTime = fields[2],
            StopId = fields[3],
            StopSequence = int.Parse(fields[4]),
            StopHeadsign = fields.Length > 5 ? fields[5] : "",
            PickupType = fields.Length > 6 && !string.IsNullOrEmpty(fields[6]) ? int.Parse(fields[6]) : null,
            DropOffType = fields.Length > 7 && !string.IsNullOrEmpty(fields[7]) ? int.Parse(fields[7]) : null,
            ShapeDistTraveled = fields.Length > 8 && !string.IsNullOrEmpty(fields[8]) ?
                double.Parse(fields[8], System.Globalization.CultureInfo.InvariantCulture) : null
        });
    }
}