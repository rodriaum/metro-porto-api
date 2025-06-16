﻿using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api;
using Microsoft.AspNetCore.Mvc;

namespace MetroPortoAPI.Api.Controller.System;

[ApiController]
[Route("api/v1/transit/gtfs")]
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
			message = "ok",
            healthy = true,
            timestamp = DateTime.UtcNow
        });
    }
}