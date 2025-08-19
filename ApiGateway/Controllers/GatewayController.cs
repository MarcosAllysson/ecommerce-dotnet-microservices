using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    private readonly GatewayService _gatewayService;
    private readonly IConfiguration _configuration;

    public GatewayController(GatewayService gatewayService, IConfiguration configuration)
    {
        _gatewayService = gatewayService;
        _configuration = configuration;
    }

    [HttpPost("route")]
    public async Task<IActionResult> RouteRequest([FromBody] Request request)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var response = await _gatewayService.RouteRequestAsync(request, token);

        if (response.Success)
            return Ok(response.Data);

        return BadRequest(new { error = response.ErrorMessage });
    }

    [AllowAnonymous]
    [HttpGet("generate-token")]
    public IActionResult GenerateToken()
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "admin")
            }),

            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { token = tokenString });
    }
}