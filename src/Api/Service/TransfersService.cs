using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

public class TransfersService : MongoService<Transfer>, ITransfersService
{
    private readonly IRedisService _redis;

    public TransfersService(IMongoDatabase database, ILogger<TransfersService> logger, IRedisService redis)
            : base(database, logger, "transfers")
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
            FromStopId = fields[0],
            ToStopId = fields[1],
            TransferType = int.Parse(fields[2])
        });
    }
}