using TransitGtfs.Api.Interfaces;
using TransitGtfs.Api.Interfaces.Database;
using TransitGtfs.Api.Models;
using TransitGtfs.Api.Service.Database;
using TransitGtfs.Api.Utils;
using MongoDB.Driver;
using System.Globalization;

namespace TransitGtfs.Api.Service;

public class ShapesService : MongoService<Shape>, IShapesService
{
    private readonly IRedisService _redis;

    public ShapesService(IMongoDatabase database, ILogger<ShapesService> logger, IRedisService redis)
        : base(database, logger, "gtfs_shapes")
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
            ShapeId = fields.GetValueOrDefault("shape_id", "") ?? "",
            ShapePtLat = NumberUtil.ParseDoubleSafe(fields.GetValueOrDefault("shape_pt_lat", null), format: CultureInfo.InvariantCulture),
            ShapePtLon = NumberUtil.ParseDoubleSafe(fields.GetValueOrDefault("shape_pt_lon", null), format: CultureInfo.InvariantCulture),
            ShapePtSequence = NumberUtil.ParseIntSafe(fields.GetValueOrDefault("shape_pt_sequence", null)),
            ShapeDistTraveled = NumberUtil.ParseDoubleSafe(fields.GetValueOrDefault("shape_dist_traveled", null), format: CultureInfo.InvariantCulture),
        });
    }
}