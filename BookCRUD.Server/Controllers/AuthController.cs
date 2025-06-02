using Microsoft.AspNetCore.Mvc;
using BookCRUD.Server.Services;
using BookCRUD.Server.Models;
using BookCRUD.Server.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http; // Required for StatusCodes

namespace BookCRUD.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly InMemoryUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(InMemoryUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null || string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _userService.FindUserByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var userRole = string.IsNullOrWhiteSpace(registerDto.Role) ? "User" : registerDto.Role;
            var user = await _userService.AddUserAsync(registerDto.Username, registerDto.Password, userRole);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.");
            }
            // Typically, you wouldn't return the user object directly, especially with password hash.
            // For this example, returning a simple success message or a user DTO (without sensitive info) is better.
            return Ok(new { Message = "User registered successfully", UserId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var isValidCredentials = await _userService.ValidateCredentialsAsync(loginDto.Username, loginDto.Password);
            if (!isValidCredentials)
            {
                return Unauthorized("Invalid credentials.");
            }

            var user = await _userService.FindUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                 // This case should ideally not be hit if ValidateCredentialsAsync is true,
                 // but as a safeguard:
                return Unauthorized("User not found after validation.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new TokenDto { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var secretKeyString = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKeyString))
            {
                // Log this error or handle it appropriately
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Common way to store User ID
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured.");
            }
            if (string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT Audience is not configured.");
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120), // Token expiration time
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
