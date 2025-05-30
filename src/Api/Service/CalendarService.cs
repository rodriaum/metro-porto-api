using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service;

public class CalendarService : MongoService<Calendar>, ICalendarService
{
    private readonly IRedisService _redis;

    public CalendarService(IMongoDatabase database, ILogger<CalendarService> logger, IRedisService redis)
        : base(database, logger, "calendar")
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
            ServiceId = fields[0],
            Monday = int.Parse(fields[1]),
            Tuesday = int.Parse(fields[2]),
            Wednesday = int.Parse(fields[3]),
            Thursday = int.Parse(fields[4]),
            Friday = int.Parse(fields[5]),
            Saturday = int.Parse(fields[6]),
            Sunday = int.Parse(fields[7]),
            StartDate = fields[8],
            EndDate = fields[9]
        });
    }
}