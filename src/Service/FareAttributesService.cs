using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Models;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;
using MongoDB.Driver;
using System.Globalization;

namespace TransitGtfsApi.Service;

public class FareAttributesService : MongoService<FareAttribute>, IFareAttributesService
{
    private readonly IRedisService _redis;

    public FareAttributesService(IMongoDatabase database, ILogger<FareAttributesService> logger, IRedisService redis)
        : base(database, logger, "gtfs_fare_attributes")
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
            FareId = fields.GetValueOrDefault("fare_id", "") ?? "",
            Price = NumberUtil.ParseDecimalSafe(fields.GetValueOrDefault("price", null), format: CultureInfo.InvariantCulture),
            CurrencyType = fields.GetValueOrDefault("currency_type", "") ?? "",
            PaymentMethod = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("payment_method", null)),
            Transfers = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("transfers", null)),
            TransferDuration = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("transfer_duration", null)),
        });
    }
}