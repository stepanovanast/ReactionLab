namespace ReactionLab.Data.Models;

public class UserProgress
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    public ProgressStatus Status { get; set; } = ProgressStatus.Available;
    public DateTime? CompletedAt { get; set; }
}

public enum ProgressStatus
{
    Locked,
    Available,
    Current,
    Completed
}
