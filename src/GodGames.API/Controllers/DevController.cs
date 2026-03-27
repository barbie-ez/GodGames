using GodGames.Application.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace GodGames.API.Controllers;

/// <summary>
/// Development-only endpoints. Returns 404 outside of Development environment.
/// </summary>
[ApiController]
[Route("api/dev")]
public class DevController(WorldTickJob tickJob, IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// Immediately runs the world tick without waiting for the 6-hour schedule.
    /// Only available in Development environment.
    /// </summary>
    [HttpPost("tick")]
    public async Task<IActionResult> TriggerTick()
    {
        if (!env.IsDevelopment() && !env.IsEnvironment("Testing"))
            return NotFound();

        await tickJob.ExecuteAsync();
        return Ok(new { message = "Tick completed." });
    }
}
