using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;
using ApiGateway.Services;

namespace ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    private readonly GatewayService _gatewayService;

    public GatewayController(GatewayService gatewayService)
    {
        _gatewayService = gatewayService;
    }

    [HttpPost("route")]
    public async Task<IActionResult> RouteRequest([FromBody] Request request)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var response = await _gatewayService.RouteRequestAsync(request, token);
        if (response.Success)
            return Ok(response.Data);
        else
            return BadRequest(new { error = response.ErrorMessage });
    }
}