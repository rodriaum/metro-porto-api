using TransitGtfs.Api.Interfaces;
using TransitGtfs.Api.Interfaces.Database;
using TransitGtfs.Api.Models;
using TransitGtfs.Api.Service.Database;
using MongoDB.Driver;

namespace TransitGtfs.Api.Service;

public class AgencyService : MongoService<Agency>, IAgencyService
{
    private readonly IRedisService _redis;

    public AgencyService(IMongoDatabase database, ILogger<AgencyService> logger, IRedisService redis)
        : base(database, logger, "gtfs_agency")
    {
        _redis = redis;

        IndexKeysDefinition<Agency> indexKeysDefinition = Builders<Agency>.IndexKeys.Ascending(a => a.AgencyId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Agency>(indexKeysDefinition));
    }

    public async Task<List<Agency>> GetAllAsync()
    {
        return await _collection.Find(Builders<Agency>.Filter.Empty).ToListAsync();
    }

    public async Task<Agency?> GetByIdAsync(string agencyId)
    {
        return await _redis.GetOrSetAsync(
            $"agency-{agencyId}",
            async () => await _collection.Find(a => a.AgencyId == agencyId).FirstOrDefaultAsync()
        );
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "agency.txt");
        await ImportFromCsvAsync(filePath, fields => new Agency
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            AgencyId = fields.GetValueOrDefault("agency_id", "") ?? "",
            AgencyName = fields.GetValueOrDefault("agency_name", "") ?? "",
            AgencyUrl = fields.GetValueOrDefault("agency_url", "") ?? "",
            AgencyTimezone = fields.GetValueOrDefault("agency_timezone", "") ?? "",
            AgencyLang = fields.GetValueOrDefault("agency_lang", "") ?? ""
        });
    }
}