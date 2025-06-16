using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service.Database;
using MetroPortoAPI.Api.Utils;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

public class CalendarService : MongoService<Calendar>, ICalendarService
{
    private readonly IRedisService _redis;

    public CalendarService(IMongoDatabase database, ILogger<CalendarService> logger, IRedisService redis)
        : base(database, logger, "gtfs_calendar")
    {
        _redis = redis;

        IndexKeysDefinition<Calendar> indexKeysDefinition = Builders<Calendar>.IndexKeys.Ascending(c => c.ServiceId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Calendar>(indexKeysDefinition));
    }

    public async Task<List<Calendar>> GetAllAsync()
    {
        return await _collection.Find(Builders<Calendar>.Filter.Empty).ToListAsync();
    }

    public async Task<Calendar?> GetByIdAsync(string serviceId)
    {
        return await _redis.GetOrSetAsync(
            $"calendar-{serviceId}",
            async () => await _collection.Find(c => c.ServiceId == serviceId).FirstOrDefaultAsync()
        );
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "calendar.txt");

        await ImportFromCsvAsync(filePath, fields => new Calendar
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            ServiceId = fields.GetValueOrDefault("service_id", "") ?? "",
            Monday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("monday", null)),
            Tuesday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("tuesday", null)),
            Wednesday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("wednesday", null)),
            Thursday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("thursday", null)),
            Friday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("friday", null)),
            Saturday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("saturday", null)),
            Sunday = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("sunday", null)),
            StartDate = fields.GetValueOrDefault("start_date", "") ?? "",
            EndDate = fields.GetValueOrDefault("end_date", "") ?? "",
        });
    }
}