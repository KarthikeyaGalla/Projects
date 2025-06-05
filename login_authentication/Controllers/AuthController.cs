using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using login_authentication.DTO;
using login_authentication.Models;
using login_authentication.Helpers;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;

namespace login_authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TableClient _tableClient;
        private readonly jwtsettings _jwtsettings;
        public AuthController(TableServiceClient serviceClient, jwtsettings jwtsettings)
        {
            _tableClient = serviceClient.GetTableClient("Users");
            _tableClient.CreateIfNotExists();
            _jwtsettings = jwtsettings;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            var existingUser = await _tableClient.GetEntityIfExistsAsync<UserEntity>("Users", dto.Username);
            if (existingUser.HasValue)
                return BadRequest("User already exists");

            var hashedPassword = PasswordHashing.HashPassword(dto.Password);

            var newUser = new UserEntity
            {
                RowKey = dto.Username,
                PasswordHash = hashedPassword,
                Email = dto.Email
            };

            await _tableClient.AddEntityAsync(newUser);
            return Ok("User registered successfully");
        }

        //[HttpPost]
        //[Route("token")]
        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _tableClient.GetEntityIfExistsAsync<UserEntity>("Users", dto.Username);
            if (!result.HasValue)
                return Unauthorized("Invalid credentials");

            var user = result.Value;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Username),
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtsettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtsettings.Issuer,
                audience: _jwtsettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtsettings.ExpiryMinutes),
                signingCredentials: creds
            );
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
