using MongoDB.Driver;
using TransitGtfsApi.Enums;
using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Models;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Utils;

namespace TransitGtfsApi.Service;

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

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {FilePath}", filePath);
            return;
        }

        await ImportFromCsvAsync(filePath, fields =>
        {
            int mondayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("monday", null), 0);
            int tuesdayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("tuesday", null), 0);
            int wednesdayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("wednesday", null), 0);
            int thursdayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("thursday", null), 0);
            int fridayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("friday", null), 0);
            int saturdayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("saturday", null), 0);
            int sundayValue = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("sunday", null), 0);

            if (!EnumUtil.TryFromValue(mondayValue, out StatusType monday))
            {
                _logger.LogWarning("Invalid monday value: {mondayValue}. Using default value.", mondayValue);
                monday = StatusType.Inactive;
            }

            if (!EnumUtil.TryFromValue(tuesdayValue, out StatusType tuesday))
            {
                _logger.LogWarning("Invalid tuesday value: {tuesdayValue}. Using default value.", tuesdayValue);
                tuesday = StatusType.Inactive;
            }

            if (!EnumUtil.TryFromValue(wednesdayValue, out StatusType wednesday))
            {
                _logger.LogWarning("Invalid wednesday value: {wednesdayValue}. Using default value.", wednesdayValue);
                wednesday = StatusType.Inactive;
            }

            if (!EnumUtil.TryFromValue(thursdayValue, out StatusType thursday))
            {
                _logger.LogWarning("Invalid thursday value: {thursdayValue}. Using default value.", thursdayValue);
                thursday = StatusType.Inactive;
            }

            if (!EnumUtil.TryFromValue(fridayValue, out StatusType friday))
            {
                _logger.LogWarning("Invalid friday value: {fridayValue}. Using default value.", fridayValue);
                friday = StatusType.Inactive;
            }

            if (!EnumUtil.TryFromValue(saturdayValue, out StatusType saturday))
            {
                _logger.LogWarning("Invalid saturday value: {saturdayValue}. Using default value.", saturdayValue);
                saturday = StatusType.Inactive;
            }

            if (!EnumUtil.TryFromValue(sundayValue, out StatusType sunday))
            {
                _logger.LogWarning("Invalid sunday value: {sundayValue}. Using default value.", sundayValue);
                sunday = StatusType.Inactive;
            }

            return new Calendar
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                ServiceId = fields.GetValueOrDefault("service_id", "") ?? "",
                Monday = monday,
                Tuesday = tuesday,
                Wednesday = wednesday,
                Thursday = thursday,
                Friday = friday,
                Saturday = saturday,
                Sunday = sunday,
                StartDate = fields.GetValueOrDefault("start_date", "") ?? "",
                EndDate = fields.GetValueOrDefault("end_date", "") ?? "",
            };
        });
    }
}