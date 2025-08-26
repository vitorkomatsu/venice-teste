using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Venice.Teste.Backend.WebApi.DTOs;

namespace Venice.Teste.Backend.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateToken([FromBody] TokenRequest request)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? "venice.local";
            var audience = _configuration["Jwt:Audience"] ?? "venice.api";
            var secret = _configuration["Jwt:Secret"] ?? "super_secret_dev_key_please_change";

            if (string.IsNullOrWhiteSpace(secret))
            {
                return BadRequest("JWT secret not configured.");
            }

            var sub = string.IsNullOrWhiteSpace(request?.Sub) ? "tester" : request!.Sub!;
            var name = string.IsNullOrWhiteSpace(request?.Name) ? "Local Dev" : request!.Name!;
            var expMinutes = request?.ExpMinutes is > 0 ? request!.ExpMinutes!.Value : 60; // default 60 minutos

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(expMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, sub),
                new(JwtRegisteredClaimNames.UniqueName, name),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                expiresAt = expires.ToString("o"),
                issuer,
                audience
            });
        }
    }
}