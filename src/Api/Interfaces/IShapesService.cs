using MetroPorto.Api.Models;

namespace MetroPorto.Api.Interfaces;

public interface IShapesService
{
    Task<List<Shape>> GetAllAsync();
    Task<List<Shape>?> GetByShapeIdAsync(string shapeId);
    Task ImportDataAsync(string directoryPath);
}
