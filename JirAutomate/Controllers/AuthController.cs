using JirAutomate.Models;
using JirAutomate.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JirAutomate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _config;

    public AuthController(UserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.JiraDomain) ||
            string.IsNullOrWhiteSpace(user.ProjectKey) ||
            string.IsNullOrWhiteSpace(user.JiraEmail) ||
            string.IsNullOrWhiteSpace(user.JiraApi))
        {
            return BadRequest("All Jira fields are required");
        }
        if (_userService.GetByEmail(user.Email) != null)
            return BadRequest("User already exists");

        user.PasswordHash = HashPassword(user.PasswordHash);
        _userService.Create(user);
        // Fetch the user again to get the generated Id
        var createdUser = _userService.GetByEmail(user.Email);
        Console.WriteLine($"[REGISTER] Created user: Email={createdUser.Email}, Id={createdUser.Id}");
        var token = GenerateJwtToken(createdUser);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginUser)
    {
        var user = _userService.GetByEmail(loginUser.Email);
        if (user == null || !VerifyPassword(loginUser.PasswordHash, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        // Fetch the user again to ensure Id is set (in case of legacy data)
        var dbUser = _userService.GetByEmail(loginUser.Email);
        Console.WriteLine($"[LOGIN] User: Email={dbUser.Email}, Id={dbUser.Id}");
        var token = GenerateJwtToken(dbUser);
        return Ok(new { token });
    }

    // ----------------- Helpers -----------------

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("id", user.Id ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
