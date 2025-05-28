using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Interfaces.Database;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class AgencyService : MongoService<Agency>, IAgencyService
{
    private readonly IRedisService _redis;

    public AgencyService(IMongoDatabase database, ILogger<AgencyService> logger, IRedisService redis)
        : base(database, logger, "agency")
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
            AgencyId = fields[0],
            AgencyName = fields[1],
            AgencyUrl = fields[2],
            AgencyTimezone = fields[3],
            AgencyLang = fields.Length > 4 ? fields[4] : ""
        });
    }
}