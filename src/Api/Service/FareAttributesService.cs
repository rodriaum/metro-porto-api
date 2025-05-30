using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

public class FareAttributesService : MongoService<FareAttribute>, IFareAttributesService
{
    private readonly IRedisService _redis;

    public FareAttributesService(IMongoDatabase database, ILogger<FareAttributesService> logger, IRedisService redis)
        : base(database, logger, "fare_attributes")
    {
        _redis = redis;

        IndexKeysDefinition<FareAttribute> indexKeysDefinition = Builders<FareAttribute>.IndexKeys.Ascending(r => r.FareId);
        _collection.Indexes.CreateOne(new CreateIndexModel<FareAttribute>(indexKeysDefinition));
    }

    public async Task<List<FareAttribute>> GetAllAsync()
    {
        return await _collection.Find(Builders<FareAttribute>.Filter.Empty).ToListAsync();
    }

    public async Task<FareAttribute?> GetByIdAsync(string fareId)
    {
        return await _redis.GetOrSetAsync(
            $"fare-attributes-{fareId}",
            async () => await _collection.Find(f => f.FareId == fareId).FirstOrDefaultAsync()
        );
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "fare_attributes.txt");
        await ImportFromCsvAsync(filePath, fields => new FareAttribute
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            FareId = fields[0],
            Price = decimal.Parse(fields[1], System.Globalization.CultureInfo.InvariantCulture),
            CurrencyType = fields[2],
            PaymentMethod = int.Parse(fields[3]),
            Transfers = int.Parse(fields[4]),
            TransferDuration = fields.Length > 5 && !string.IsNullOrEmpty(fields[5]) ? int.Parse(fields[5]) : (int?)null
        });
    }
}