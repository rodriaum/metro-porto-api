using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class TripsService : MongoService<Trip>, ITripsService
{
    public TripsService(IMongoDatabase database, ILogger<TripsService> logger)
        : base(database, logger, "trips")
    {
    }

    public async Task<List<Trip>> GetAllAsync()
    {
        return await _collection.Find(Builders<Trip>.Filter.Empty).ToListAsync();
    }

    public async Task<Trip> GetByIdAsync(string tripId)
    {
        return await _collection.Find(t => t.TripId == tripId).FirstOrDefaultAsync();
    }

    public async Task<List<Trip>> GetByRouteIdAsync(string routeId)
    {
        return await _collection.Find(t => t.RouteId == routeId).ToListAsync();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "trips.txt");
        await ImportFromCsvAsync(filePath, fields => new Trip
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            RouteId = fields[0],
            ServiceId = fields[1],
            TripId = fields[2],
            TripHeadsign = fields.Length > 3 ? fields[3] : null,
            DirectionId = fields.Length > 4 && !string.IsNullOrEmpty(fields[4]) ? int.Parse(fields[4]) : (int?)null,
            BlockId = fields.Length > 5 ? fields[5] : null,
            ShapeId = fields.Length > 6 ? fields[6] : null
        });
    }
}