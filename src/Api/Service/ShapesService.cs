using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class ShapesService : BaseGtfsService<Shape>, IShapesService
{
    public ShapesService(IMongoDatabase database, ILogger<ShapesService> logger)
        : base(database, logger, "shapes")
    {
    }

    public async Task<List<Shape>> GetAllAsync()
    {
        return await _collection.Find(Builders<Shape>.Filter.Empty).ToListAsync();
    }

    public async Task<List<Shape>> GetByShapeIdAsync(string shapeId)
    {
        return await _collection.Find(s => s.ShapeId == shapeId).ToListAsync();
    }

    public async Task ImportDataAsync(string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, "shapes.txt");
        await ImportFromCsvAsync(filePath, fields => new Shape
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            ShapeId = fields[0],
            ShapePtLat = double.Parse(fields[1], System.Globalization.CultureInfo.InvariantCulture),
            ShapePtLon = double.Parse(fields[2], System.Globalization.CultureInfo.InvariantCulture),
            ShapePtSequence = int.Parse(fields[3]),
            ShapeDistTraveled = fields.Length > 4 && !string.IsNullOrEmpty(fields[4]) ?
                double.Parse(fields[4], System.Globalization.CultureInfo.InvariantCulture) : (double?)null
        });
    }
}