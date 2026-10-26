using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RearqBim.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simulación B2B: Validación de credenciales de prueba
            var role = "PROJECT_RESIDENT";
            var companyId = "11111111-1111-1111-1111-111111111111";
            var companyName = "Constructora Bolívar S.A.";
            var userId = "55555555-5555-5555-5555-555555555555";

            if (request.Email.Contains("gomez", StringComparison.OrdinalIgnoreCase))
            {
                role = "BIM_MODELER";
                companyId = "22222222-2222-2222-2222-222222222222";
                companyName = "Arquitectura & Concretos Medellín S.A.S.";
                userId = "66666666-6666-6666-6666-666666666666";
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("REARQ_BIM_SUPER_SECRET_KEY_FOR_LOCAL_DEVELOPMENT_2026_JWT_TOKEN");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("CompanyId", companyId),
                    new Claim("CompanyName", companyName)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = jwtString,
                role = role,
                companyId = companyId,
                companyName = companyName,
                hasBimSubscription = true
            });
        }
    }
}
