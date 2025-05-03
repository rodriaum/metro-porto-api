using MetroPorto.Api.Interfaces;
using MetroPorto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.Metro;

[ApiController]
[Route("v1/porto/metro")]
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