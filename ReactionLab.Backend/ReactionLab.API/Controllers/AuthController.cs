using Microsoft.AspNetCore.Mvc;
using ReactionLab.API.Services;

namespace ReactionLab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    // Constructor injection - ASP.NET automatically provides AuthService
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // =====================================================
    // POST /api/auth/signup
    // Creates a new user account
    // =====================================================
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Name, email, and password are required!" });
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(new { error = "The password must be at least 6 characters long!" });
        }

        // Attempt signup
        var result = await _authService.SignupAsync(request.Name, request.Email, request.Password, request.AvatarId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        // Return token and user info
        return Ok(new
        {
            token = result.Token,
            user = result.User
        });
    }

    // =====================================================
    // POST /api/auth/login
    // Authenticates user and returns JWT token
    // =====================================================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Email and password are required!" });
        }

        // Attempt login
        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.IsSuccess)
        {
            // Return 401 Unauthorized for invalid credentials
            return Unauthorized(new { error = result.Error });
        }

        // Return token and user info
        return Ok(new
        {
            token = result.Token,
            user = result.User
        });
    }

    // =====================================================
    // POST /api/auth/forgot
    // Initiates password reset (placeholder for now)
    // =====================================================
    [HttpPost("forgot")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        // In a real app, you would:
        // 1. Generate a reset token
        // 2. Send an email with a reset link
        // 3. Store the token with an expiration

        // For now, just acknowledge the request
        return Ok(new { message = "If this email exists, a reset link will be sent" });
    }

    // =====================================================
    // POST /api/auth/logout
    // Client-side logout (just clear the token)
    // =====================================================
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT tokens are stateless - the client just needs to delete the token
        // In a more advanced setup, you could maintain a blacklist of revoked tokens
        return Ok(new { message = "Logged out successfully" });
    }
}

// Request DTOs (Data Transfer Objects)
public record SignupRequest(string Name, string Email, string Password, int AvatarId = 1);
public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
