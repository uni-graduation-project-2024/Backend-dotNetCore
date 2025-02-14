using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;


namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var userInData = await _authRepository.Login(loginDto.Email, loginDto.Password);
            if (userInData == null)
                return Unauthorized();

            var token = GenerateJwtToken(userInData);
            return Ok(new { token });
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            var securityKey = new SymmetricSecurityKey(key);

            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpPost("registeruser")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto RegisterDto)
        {
            if (RegisterDto == null) return BadRequest();

            RegisterDto.Email = RegisterDto.Email.ToLower();
            if (await _authRepository.UserExist(RegisterDto.Email))
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                Email = RegisterDto.Email,
                Username = RegisterDto.Username,
                JoinedDate = DateTime.Now,
            };

            await _authRepository.Register(user, RegisterDto.Password);

            return Ok();
        }

    }
}
