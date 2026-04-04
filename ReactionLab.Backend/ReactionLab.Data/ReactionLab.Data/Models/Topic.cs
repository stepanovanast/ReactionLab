namespace ReactionLab.Data.Models;

public class Topic
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }

    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
