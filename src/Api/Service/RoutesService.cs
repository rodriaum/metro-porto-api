using Api.Interfaces;
using MongoDB.Driver;

namespace Api.Service;

public class RoutesService : BaseGtfsService<Api.Models.Route>, IRoutesService
{
    public RoutesService(IMongoDatabase database, ILogger<RoutesService> logger)
        : base(database, logger, "routes")
    {
    }

    public async Task<List<Api.Models.Route>> GetAllAsync()
    {
        return await _collection.Find(Builders<Api.Models.Route>.Filter.Empty).ToListAsync();
    }

    public async Task<Api.Models.Route> GetByIdAsync(string routeId)
    {
        return await _collection.Find(r => r.RouteId == routeId).FirstOrDefaultAsync();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "routes.txt");
        await ImportFromCsvAsync(filePath, fields => new Api.Models.Route
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            RouteId = fields[0],
            AgencyId = fields.Length > 1 ? fields[1] : null,
            RouteShortName = fields.Length > 2 ? fields[2] : null,
            RouteLongName = fields.Length > 3 ? fields[3] : null,
            RouteDesc = fields.Length > 4 ? fields[4] : null,
            RouteType = int.Parse(fields[5]),
            RouteUrl = fields.Length > 6 ? fields[6] : null,
            RouteColor = fields.Length > 7 ? fields[7] : null,
            RouteTextColor = fields.Length > 8 ? fields[8] : null
        });
    }
}