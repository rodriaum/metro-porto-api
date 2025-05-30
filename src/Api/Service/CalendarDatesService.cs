using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

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