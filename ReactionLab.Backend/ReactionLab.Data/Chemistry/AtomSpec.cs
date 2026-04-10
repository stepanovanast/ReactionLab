namespace ReactionLab.Data.Chemistry;

/// <summary>
/// Defines a single atom in a molecule — its ID used for referencing in bonds/transfers,
/// its chemical symbol, and its oxidation state (0 = neutral, +2 = Fe²⁺, -2 = S²⁻).
/// </summary>
public record AtomSpec(string Id, string Element, int OxidationState = 0);
