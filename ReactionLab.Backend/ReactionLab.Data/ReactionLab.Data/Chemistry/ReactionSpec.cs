namespace ReactionLab.Data.Chemistry;

/// <summary>
/// The full chemistry definition for a reaction — just its steps.
/// Pass this to StepJsonBuilder.Build() to get the JSON string for the database.
/// </summary>
public record ReactionSpec(List<ReactionStepSpec> Steps);
