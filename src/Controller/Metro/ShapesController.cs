using TransitGtfs.Api.Models;
using TransitGtfs.Api.Interfaces;
using TransitGtfs.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace TransitGtfs.Api.Controller.Metro;

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