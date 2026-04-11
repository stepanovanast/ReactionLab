using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactionLab.Data;
using ReactionLab.Data.Models;

namespace ReactionLab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TopicsController(AppDbContext context)
    {
        _context = context;
    }

    // =====================================================
    // PUBLIC ENDPOINTS (anyone can read)
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> GetTopics([FromQuery] string? search)
    {
        var query = _context.Topics.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search));

        var topics = await query.OrderBy(t => t.Order).ToListAsync();

        // Try to read user ID from the JWT if present (endpoint is public but auth-aware)
        int? userId = null;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedId))
            userId = parsedId;

        List<UserProgress> userProgress = new();
        if (userId.HasValue)
        {
            userProgress = await _context.UserProgress
                .Where(p => p.UserId == userId.Value)
                .ToListAsync();
        }

        var result = topics.Select(t =>
        {
            var progress = userProgress.FirstOrDefault(p => p.TopicId == t.Id);
            var status = progress != null
                ? progress.Status.ToString().ToLower()
                : (t.Order == 1 ? "available" : "locked"); // default: first topic open, rest locked
            return new { t.Id, t.Title, t.Description, Status = status };
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTopic(int id)
    {
        var topic = await _context.Topics
            .Include(t => t.Reactions)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (topic == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            topic.Id,
            topic.Title,
            topic.Description,
            Reactions = topic.Reactions.Select(r => new
            {
                r.Id,
                r.Title,
                r.Equation,
                r.Temperature,
                Molecules = System.Text.Json.JsonSerializer.Deserialize<object>(r.MoleculeData),
                Steps = System.Text.Json.JsonSerializer.Deserialize<object>(r.StepsData)
            })
        });
    }

    // =====================================================
    // ADMIN ENDPOINTS (admin only)
    // =====================================================

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateTopic([FromBody] TopicRequest request)
    {
        var topic = new Topic
        {
            Title = request.Title,
            Description = request.Description,
            Order = request.Order
        };

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        return Ok(new { topic.Id, topic.Title, topic.Description, topic.Order });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateTopic(int id, [FromBody] TopicRequest request)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic == null) return NotFound();

        topic.Title = request.Title;
        topic.Description = request.Description;
        topic.Order = request.Order;

        await _context.SaveChangesAsync();

        return Ok(new { topic.Id, topic.Title, topic.Description, topic.Order });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic == null) return NotFound();

        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Topic '{topic.Title}' deleted" });
    }

    [HttpPost("{id}/reactions")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateReaction(int id, [FromBody] ReactionRequest request)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic == null) return NotFound(new { error = "Topic not found" });

        var reaction = new Reaction
        {
            TopicId = id,
            Title = request.Title,
            Equation = request.Equation,
            Temperature = request.Temperature,
            MoleculeData = request.MoleculeData ?? "[]",
            StepsData = request.StepsData ?? "[]"
        };

        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();

        return Ok(new { reaction.Id, reaction.Title, reaction.Equation, reaction.Temperature });
    }
}

public record TopicRequest(string Title, string Description, int Order);
public record ReactionRequest(string Title, string Equation, double Temperature, string? MoleculeData, string? StepsData);
