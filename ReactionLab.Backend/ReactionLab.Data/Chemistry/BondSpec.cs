namespace ReactionLab.Data.Chemistry;

/// <summary>
/// A bond between two atoms, referenced by their IDs.
/// Type: "covalent", "ionic", or "metallic" — informational for now, used for future rendering hints.
/// </summary>
public record BondSpec(string From, string To, string Type = "covalent");
