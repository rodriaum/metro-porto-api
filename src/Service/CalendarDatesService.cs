using MongoDB.Driver;
using TransitGtfsApi.Enums;
using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Models;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;

namespace TransitGtfsApi.Service;

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

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {FilePath}", filePath);
            return;
        }

        await ImportFromCsvAsync(filePath, fields =>
        {
            int exceptionId = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("exception_type", null), -1);

            if (!EnumUtil.TryFromValue(exceptionId, out ExceptionType exceptionType))
            {
                _logger.LogWarning("Invalid exception type: {ExceptionId}", exceptionId);
                return null;
            }

            return new CalendarDate
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                ServiceId = fields.GetValueOrDefault("service_id", "") ?? "",
                Date = fields.GetValueOrDefault("date", "") ?? "",
                ExceptionType = exceptionType
            };
        });
    }
}