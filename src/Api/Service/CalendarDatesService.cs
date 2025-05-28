using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Interfaces.Database;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class CalendarDatesService : MongoService<CalendarDate>, ICalendarDatesService
{
    private readonly IRedisService _redis;

    public CalendarDatesService(IMongoDatabase database, ILogger<CalendarDatesService> logger, IRedisService redis)
        : base(database, logger, "calendar_dates")
    {
        _redis = redis;

        IndexKeysDefinition<CalendarDate> indexKeysDefinition = Builders<CalendarDate>.IndexKeys.Ascending(c => c.ServiceId);
        _collection.Indexes.CreateOne(new CreateIndexModel<CalendarDate>(indexKeysDefinition));
    }

    public async Task<List<CalendarDate>> GetAllAsync()
    {
        return await _collection.Find(Builders<CalendarDate>.Filter.Empty).ToListAsync();
    }

    public async Task<List<CalendarDate>?> GetByServiceIdAsync(string serviceId)
    {
        return await _redis.GetOrSetAsync(
            $"calendar-dates-service-{serviceId}",
            async () => await _collection.Find(c => c.ServiceId == serviceId).ToListAsync()
        ) ?? new List<CalendarDate>();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "calendar_dates.txt");
        await ImportFromCsvAsync(filePath, fields => new CalendarDate
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            ServiceId = fields[0],
            Date = fields[1],
            ExceptionType = int.Parse(fields[2])
        });
    }
}

public class FareAttributesService : MongoService<FareAttribute>, IFareAttributesService
{
    public FareAttributesService(IMongoDatabase database, ILogger<FareAttributesService> logger)
        : base(database, logger, "fareAttributes")
    {
    }

    public async Task<List<FareAttribute>> GetAllAsync()
    {
        return await _collection.Find(Builders<FareAttribute>.Filter.Empty).ToListAsync();
    }

    public async Task<FareAttribute> GetByIdAsync(string fareId)
    {
        return await _collection.Find(f => f.FareId == fareId).FirstOrDefaultAsync();
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