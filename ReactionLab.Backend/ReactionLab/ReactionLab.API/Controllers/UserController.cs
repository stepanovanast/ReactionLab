using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactionLab.API.Services;
using ReactionLab.Data;

namespace ReactionLab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // All endpoints here require a valid JWT token
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly BadgeService _badgeService;

    public UserController(AppDbContext context, BadgeService badgeService)
    {
        _context = context;
        _badgeService = badgeService;
    }

    // =====================================================
    // GET /api/user/profile
    // Returns the current user's profile
    // =====================================================
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Get user ID from JWT token claims
        // The token was validated by the [Authorize] attribute
        var userId = GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        // Fetch user from database
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        return Ok(new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            createdAt = user.CreatedAt
        });
    }

    // =====================================================
    // GET /api/user/badges
    // Returns the user's earned badges
    // =====================================================
    [HttpGet("badges")]
    public async Task<IActionResult> GetBadges()
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        // Get all badges with earned status for this user
        var badges = await _context.Badges
            .Select(badge => new
            {
                id = badge.Id,
                name = badge.Name,
                description = badge.Description,
                earned = badge.UserBadges.Any(ub => ub.UserId == userId)
            })
            .ToListAsync();

        return Ok(badges);
    }

    // =====================================================
    // PUT /api/user/progress
    // Updates user's progress on a topic
    // =====================================================
    [HttpPut("progress")]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        // Find existing progress or create new
        var progress = await _context.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.TopicId == request.TopicId);

        if (progress == null)
        {
            progress = new Data.Models.UserProgress
            {
                UserId = userId.Value,
                TopicId = request.TopicId
            };
            _context.UserProgress.Add(progress);
        }

        // Update status
        progress.Status = Enum.Parse<Data.Models.ProgressStatus>(request.Status, ignoreCase: true);

        if (progress.Status == Data.Models.ProgressStatus.Completed)
        {
            progress.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Check and award any newly earned completion badges
        if (progress.Status == Data.Models.ProgressStatus.Completed)
            await _badgeService.CheckCompletionBadgesAsync(userId.Value, request.TopicId);

        return Ok(new { message = "Progress updated" });
    }

    // =====================================================
    // POST /api/user/badges/molecular-vision
    // Awards Molecular Vision badge when user switches
    // visualization mode for the first time
    // =====================================================
    [HttpPost("badges/molecular-vision")]
    public async Task<IActionResult> AwardMolecularVision()
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
            return Unauthorized(new { error = "Invalid token" });

        await _badgeService.AwardMolecularVisionAsync(userId.Value);
        return Ok(new { message = "Badge awarded" });
    }

    // =====================================================
    // POST /api/user/badges/full-circuit
    // Awards Full Circuit badge when user reaches the
    // final step of any reaction
    // =====================================================
    [HttpPost("badges/full-circuit")]
    public async Task<IActionResult> AwardFullCircuit()
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
            return Unauthorized(new { error = "Invalid token" });

        await _badgeService.AwardFullCircuitAsync(userId.Value);
        return Ok(new { message = "Badge awarded" });
    }

    // =====================================================
    // Helper: Extract user ID from JWT claims
    // =====================================================
    private int? GetUserIdFromToken()
    {
        // ClaimTypes.NameIdentifier contains the user ID we stored in the token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}

public record UpdateProgressRequest(int TopicId, string Status);
