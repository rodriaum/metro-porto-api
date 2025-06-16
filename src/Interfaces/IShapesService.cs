using TransitGtfsApi.Models;

namespace TransitGtfsApi.Interfaces;

public interface IShapesService
{
    Task<List<Shape>> GetAllAsync();
    Task<List<Shape>?> GetByShapeIdAsync(string shapeId);
    Task ImportDataAsync(string directoryPath);
}
