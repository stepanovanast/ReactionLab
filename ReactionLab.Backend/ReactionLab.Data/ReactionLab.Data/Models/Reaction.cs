namespace ReactionLab.Data.Models;

public class Reaction
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Equation { get; set; } = string.Empty;
    public string MoleculeData { get; set; } = string.Empty; // JSON for molecule positions
    public string StepsData { get; set; } = string.Empty;    // JSON for reaction steps
    public double Temperature { get; set; }

    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;
}
