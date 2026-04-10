using Microsoft.EntityFrameworkCore;
using ReactionLab.Data;
using ReactionLab.Data.Models;

namespace ReactionLab.API.Services;

/// <summary>
/// Awards badges to users when they meet the required conditions.
/// All award methods are idempotent — safe to call multiple times.
///
/// Badge IDs:
///   1  Early Bird         — join ReactionLab (awarded on signup)
///   2  Iron Alchemist     — complete topic 1 (Iron Sulfide Formation)
///   3  Rising Chemist     — complete topic 2
///   4  Lab Veteran        — complete topic 3
///   5  Chemistry Master   — complete topic 4
///   6  Molecular Vision   — switch visualization mode for the first time
///   7  Full Circuit       — reach the final step of any reaction
///   8  Completionist      — complete all topics
/// </summary>
public class BadgeService
{
    private readonly AppDbContext _context;

    // Maps topicId → the badge awarded for completing that topic
    private static readonly Dictionary<int, int> TopicBadgeMap = new()
    {
        [1] = 2,   // Iron Sulfide Formation → Iron Alchemist
        [2] = 3,   // Topic 2 → Rising Chemist
        [3] = 4,   // Topic 3 → Lab Veteran
        [4] = 5,   // Topic 4 → Chemistry Master
    };

    public BadgeService(AppDbContext context)
    {
        _context = context;
    }

    // =====================================================
    // Called on signup — awards Early Bird (id 1)
    // =====================================================
    public async Task AwardEarlyBirdAsync(int userId)
    {
        await AwardIfNotEarnedAsync(userId, badgeId: 1);
        await _context.SaveChangesAsync();
    }

    // =====================================================
    // Called after a topic is marked Completed.
    // Awards the per-topic badge and the Completionist
    // badge if all topics are now done.
    // =====================================================
    public async Task CheckCompletionBadgesAsync(int userId, int completedTopicId)
    {
        // Per-topic badge
        if (TopicBadgeMap.TryGetValue(completedTopicId, out var badgeId))
            await AwardIfNotEarnedAsync(userId, badgeId);

        // Completionist — all topics done
        var totalTopics    = await _context.Topics.CountAsync();
        var completedCount = await _context.UserProgress
            .CountAsync(p => p.UserId == userId && p.Status == ProgressStatus.Completed);

        if (totalTopics > 0 && completedCount >= totalTopics)
            await AwardIfNotEarnedAsync(userId, badgeId: 8);

        await _context.SaveChangesAsync();
    }

    // =====================================================
    // Called when the user switches visualization mode
    // for the first time — awards Molecular Vision (id 6)
    // =====================================================
    public async Task AwardMolecularVisionAsync(int userId)
    {
        await AwardIfNotEarnedAsync(userId, badgeId: 6);
        await _context.SaveChangesAsync();
    }

    // =====================================================
    // Called when the user reaches the final step of any
    // reaction — awards Full Circuit (id 7)
    // =====================================================
    public async Task AwardFullCircuitAsync(int userId)
    {
        await AwardIfNotEarnedAsync(userId, badgeId: 7);
        await _context.SaveChangesAsync();
    }

    // =====================================================
    // Internal helper — inserts a UserBadge row only if
    // the user doesn't already have that badge.
    // =====================================================
    private async Task AwardIfNotEarnedAsync(int userId, int badgeId)
    {
        var alreadyEarned = await _context.UserBadges
            .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);

        if (!alreadyEarned)
            _context.UserBadges.Add(new UserBadge { UserId = userId, BadgeId = badgeId });
    }
}
