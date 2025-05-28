using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Interfaces.Database;
using MetroPorto.Api.Models;
using MetroPorto.Api.Service.Database;
using MongoDB.Driver;

namespace MetroPorto.Api.Service;

public class ShapesService : MongoService<Shape>, IShapesService
{
    private readonly IRedisService _redis;

    public ShapesService(IMongoDatabase database, ILogger<ShapesService> logger, IRedisService redis)
        : base(database, logger, "shapes")
    {
        _redis = redis;

        IndexKeysDefinition<Shape> indexKeysDefinition = Builders<Shape>.IndexKeys.Ascending(s => s.ShapeId);
        _collection.Indexes.CreateOne(new CreateIndexModel<Shape>(indexKeysDefinition));
    }

    public async Task<List<Shape>> GetAllAsync()
    {
        return await _collection.Find(Builders<Shape>.Filter.Empty).ToListAsync();
    }

    public async Task<List<Shape>?> GetByShapeIdAsync(string shapeId)
    {
        return await _redis.GetOrSetAsync(
            $"shapes-{shapeId}",
            async () => await _collection.Find(s => s.ShapeId == shapeId).ToListAsync()
        );
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