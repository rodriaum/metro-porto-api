using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class StopsService : BaseGtfsService<Stop>, IStopsService
{
    public StopsService(IMongoDatabase database, ILogger<StopsService> logger)
        : base(database, logger, "stops")
    {
    }

    public async Task<List<Stop>> GetAllAsync()
    {
        return await _collection.Find(Builders<Stop>.Filter.Empty).ToListAsync();
    }

    public async Task<Stop> GetByIdAsync(string stopId)
    {
        return await _collection.Find(s => s.StopId == stopId).FirstOrDefaultAsync();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "stops.txt");
        await ImportFromCsvAsync(filePath, fields => new Stop
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            StopId = fields[0],
            StopCode = fields.Length > 1 ? fields[1] : null,
            StopName = fields.Length > 2 ? fields[2] : null,
            StopDesc = fields.Length > 3 ? fields[3] : null,
            StopLat = double.Parse(fields[4], System.Globalization.CultureInfo.InvariantCulture),
            StopLon = double.Parse(fields[5], System.Globalization.CultureInfo.InvariantCulture),
            ZoneId = fields.Length > 6 ? fields[6] : null,
            StopUrl = fields.Length > 7 ? fields[7] : null
        });
    }
}