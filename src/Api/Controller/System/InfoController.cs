using MetroPorto.Api;
using MetroPorto.Api.Models;
using MetroPorto.Api.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MetroPorto.Api.Controller.System;

[ApiController]
[Route("v1/porto/metro")]
public class InfoController : ControllerBase
{
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        return Ok(Constant.Version);
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            status = Status.Active.ToString().ToLower(),
            healthy = true,
            timestamp = DateTime.UtcNow
        });
    }
}