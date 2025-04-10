using Api.Interfaces;
using Api.Models;
using MongoDB.Driver;

namespace Api.Service;

public class AgencyService : BaseGtfsService<Agency>, IAgencyService
{
    public AgencyService(IMongoDatabase database, ILogger<AgencyService> logger)
        : base(database, logger, "agency")
    {
    }

    public async Task<List<Agency>> GetAllAsync()
    {
        return await _collection.Find(Builders<Agency>.Filter.Empty).ToListAsync();
    }

    public async Task<Agency> GetByIdAsync(string agencyId)
    {
        return await _collection.Find(a => a.AgencyId == agencyId).FirstOrDefaultAsync();
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
            AgencyLang = fields.Length > 4 ? fields[4] : null
        });
    }
}