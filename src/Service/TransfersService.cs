using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Models;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;
using MongoDB.Driver;

namespace TransitGtfsApi.Service;

public class TransfersService : MongoService<Transfer>, ITransfersService
{
    private readonly IRedisService _redis;

    public TransfersService(IMongoDatabase database, ILogger<TransfersService> logger, IRedisService redis)
            : base(database, logger, "gtfs_transfers")
    {
        _redis = redis;

        IndexKeysDefinition<Transfer> indexKeysDefinition = Builders<Transfer>.IndexKeys.Ascending(t => t.FromStopId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Transfer>(indexKeysDefinition));

        indexKeysDefinition = Builders<Transfer>.IndexKeys.Ascending(t => t.ToStopId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Transfer>(indexKeysDefinition));
    }

    public async Task<List<Transfer>> GetAllAsync()
    {
        return await _collection.Find(Builders<Transfer>.Filter.Empty).ToListAsync();
    }

    public async Task<List<Transfer>> GetByFromStopIdAsync(string fromStopId)
    {
        return await _redis.GetOrSetAsync(
            $"transfers-from-{fromStopId}",
            async () => await _collection.Find(t => t.FromStopId == fromStopId).ToListAsync()
        ) ?? new List<Transfer>();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "transfers.txt");

        await ImportFromCsvAsync(filePath, fields => new Transfer
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            FromStopId = fields.GetValueOrDefault("from_stop_id", "") ?? "",
            ToStopId = fields.GetValueOrDefault("to_stop_id", "") ?? "",
            TransferType = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("tranfer_type", null))
        });
    }
}