using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class StopTimesService : MongoService<StopTime>, IStopTimesService
{
    public StopTimesService(IMongoDatabase database, ILogger<StopTimesService> logger)
        : base(database, logger, "stopTimes")
    {
    }

    public async Task<List<StopTime>> GetAllAsync()
    {
        return await _collection.Find(Builders<StopTime>.Filter.Empty).ToListAsync();
    }

    public async Task<List<StopTime>> GetByTripIdAsync(string tripId)
    {
        return await _collection.Find(st => st.TripId == tripId).ToListAsync();
    }

    public async Task<List<StopTime>> GetByStopIdAsync(string stopId)
    {
        return await _collection.Find(st => st.StopId == stopId).ToListAsync();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "stop_times.txt");
        await ImportFromCsvAsync(filePath, fields => new StopTime
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            TripId = fields[0],
            ArrivalTime = fields[1],
            DepartureTime = fields[2],
            StopId = fields[3],
            StopSequence = int.Parse(fields[4]),
            StopHeadsign = fields.Length > 5 ? fields[5] : null,
            PickupType = fields.Length > 6 && !string.IsNullOrEmpty(fields[6]) ? int.Parse(fields[6]) : (int?)null,
            DropOffType = fields.Length > 7 && !string.IsNullOrEmpty(fields[7]) ? int.Parse(fields[7]) : (int?)null,
            ShapeDistTraveled = fields.Length > 8 && !string.IsNullOrEmpty(fields[8]) ?
                double.Parse(fields[8], System.Globalization.CultureInfo.InvariantCulture) : (double?)null
        });
    }
}