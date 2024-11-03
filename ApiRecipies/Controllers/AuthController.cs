using ApiRecipies.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeAPI.DTO.Auth;
using RecipeAPI.Models;
using RecipeAPI.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RegisterRequest = RecipeAPI.DTO.Auth.RegisterRequest;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly  IUserService _userService;
        private readonly JwtSettings _jwtSettings;
        public AuthController(IUserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _userService.GetUserByUsername(request.Username);
            if (existingUser != null)
            {
                return BadRequest("El nombre de usuario ya esta en uso");
            }

            existingUser = await _userService.GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                return BadRequest("El correo electronico ya esta en uso");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userService.Create(user);
            return Ok("Usuario registrado exitosamente");
        }
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] DTO.Auth.LoginRequest request)
        {
            var user = await _userService.GetUserByUsername(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Nombre de usuario o contraseña incorrectos.");
            }

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
