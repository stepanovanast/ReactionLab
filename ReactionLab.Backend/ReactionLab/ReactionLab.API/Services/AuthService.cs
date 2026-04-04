using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReactionLab.Data;
using ReactionLab.Data.Models;

namespace ReactionLab.API.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly BadgeService _badgeService;

    public AuthService(AppDbContext context, IConfiguration configuration, BadgeService badgeService)
    {
        _context = context;
        _configuration = configuration;
        _badgeService = badgeService;
    }

    // =====================================================
    // SIGNUP: Create a new user
    // =====================================================
    public async Task<AuthResult> SignupAsync(string name, string email, string password)
    {
        // Step 1: Check if email already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return AuthResult.Fail("Email already registered");
        }

        // Step 2: Hash the password (NEVER store plain text passwords!)
        var passwordHash = HashPassword(password);

        // Step 3: Create and save the user
        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Step 4: Award the Early Bird badge for joining
        await _badgeService.AwardEarlyBirdAsync(user.Id);

        // Step 5: Generate a JWT token so they're logged in immediately
        var token = GenerateJwtToken(user);

        return AuthResult.Success(token, user);
    }

    // =====================================================
    // LOGIN: Verify credentials and return token
    // =====================================================
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        // Step 1: Find user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return AuthResult.Fail("Invalid email or password");
        }

        // Step 2: Verify the password
        if (!VerifyPassword(password, user.PasswordHash))
        {
            return AuthResult.Fail("Invalid email or password");
        }

        // Step 3: Generate JWT token
        var token = GenerateJwtToken(user);

        return AuthResult.Success(token, user);
    }

    // =====================================================
    // ADMIN SEEDER: Called on startup to ensure admin exists
    // =====================================================
    public async Task EnsureAdminExistsAsync()
    {
        var adminEmail = _configuration["Admin:Email"]!;
        var adminPassword = _configuration["Admin:Password"]!;

        var exists = await _context.Users.AnyAsync(u => u.IsAdmin);
        if (exists) return;

        var admin = new User
        {
            Name = "Admin",
            Email = adminEmail,
            PasswordHash = HashPassword(adminPassword),
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();
    }

    // =====================================================
    // PASSWORD HASHING
    // We use PBKDF2 - a secure one-way hashing algorithm
    // "password123" → "salt:hash" (cannot be reversed)
    // =====================================================
    private string HashPassword(string password)
    {
        // Generate a random salt (makes each hash unique)
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Hash the password with the salt (100,000 iterations for security)
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations: 100000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        // Combine salt + hash and encode as base64 string
        // Format: "base64salt:base64hash"
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        // Split the stored hash into salt and hash parts
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] originalHash = Convert.FromBase64String(parts[1]);

        // Hash the provided password with the same salt
        byte[] newHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations: 100000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        // Compare the hashes (constant-time comparison for security)
        return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
    }

    // =====================================================
    // JWT TOKEN GENERATION
    // Creates a signed token containing user info
    // =====================================================
    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        // Claims = pieces of info stored in the token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // User ID
            new Claim(ClaimTypes.Email, user.Email),                    // Email
            new Claim(ClaimTypes.Name, user.Name),                      // Name
            new Claim("IsAdmin", user.IsAdmin.ToString()),              // Admin flag
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
        };

        // Create the signing key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create the token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        // Convert to string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// =====================================================
// RESULT CLASS
// A clean way to return success/failure from auth methods
// =====================================================
public class AuthResult
{
    public bool IsSuccess { get; private set; }
    public string? Token { get; private set; }
    public string? Error { get; private set; }
    public UserDto? User { get; private set; }

    public static AuthResult Success(string token, User user) => new()
    {
        IsSuccess = true,
        Token = token,
        User = new UserDto(user.Id, user.Name, user.Email)
    };

    public static AuthResult Fail(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}

public record UserDto(int Id, string Name, string Email);
