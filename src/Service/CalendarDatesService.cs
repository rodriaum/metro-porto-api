using TransitGtfs.Api.Interfaces;
using TransitGtfs.Api.Interfaces.Database;
using TransitGtfs.Api.Models;
using TransitGtfs.Api.Service.Database;
using TransitGtfs.Api.Utils;
using MongoDB.Driver;

namespace TransitGtfs.Api.Service;

public class CalendarDatesService : MongoService<CalendarDate>, ICalendarDatesService
{
    private readonly IRedisService _redis;

    public CalendarDatesService(IMongoDatabase database, ILogger<CalendarDatesService> logger, IRedisService redis)
        : base(database, logger, "gtfs_calendar_dates")
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
            ServiceId = fields.GetValueOrDefault("service_id", "") ?? "",
            Date = fields.GetValueOrDefault("date", "") ?? "",
            ExceptionType = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("exception_type", null))
        });
    }
}