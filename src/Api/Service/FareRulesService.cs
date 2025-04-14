using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class FareRulesService : BaseGtfsService<FareRule>, IFareRulesService
{
    public FareRulesService(IMongoDatabase database, ILogger<FareRulesService> logger)
        : base(database, logger, "fareRules")
    {
    }

    public async Task<List<FareRule>> GetAllAsync()
    {
        return await _collection.Find(Builders<FareRule>.Filter.Empty).ToListAsync();
    }

    public async Task<List<FareRule>> GetByFareIdAsync(string fareId)
    {
        return await _collection.Find(f => f.FareId == fareId).ToListAsync();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "fare_rules.txt");
        await ImportFromCsvAsync(filePath, fields => new FareRule
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            FareId = fields[0],
            RouteId = fields.Length > 1 ? fields[1] : null,
            OriginId = fields.Length > 2 ? fields[2] : null,
            DestinationId = fields.Length > 3 ? fields[3] : null
        });
    }
}