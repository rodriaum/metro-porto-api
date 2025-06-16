using TransitGtfsApi.Models;
using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace TransitGtfsApi.Controller.Metro;

[ApiController]
[Route("api/v1/transit/gtfs")]
public class ShapesController : ControllerBase
{
    private readonly IShapesService _shapesService;

    public ShapesController(IShapesService shapesService)
    {
        _shapesService = shapesService;
    }

    [HttpGet("shapes")]
    public async Task<ActionResult<List<Shape>>> GetAll()
    {
        return await _shapesService.GetAllAsync();
    }

    [HttpGet("shapes/{shapeId}")]
    public async Task<ActionResult<List<Shape>>> GetByShapeId(string shapeId)
    {
        return await _shapesService.GetByShapeIdAsync(shapeId);
    }
}