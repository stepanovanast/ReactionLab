namespace ReactionLab.Data.Chemistry;

/// <summary>
/// Visual overrides for a specific atom in a step.
/// RadiusScale: shrink/grow atom to show ionization (Fe²⁺ shrinks, S²⁻ expands).
/// Dimmed: atom has lost electrons — rendered greyed out.
/// Darkened: atom is in a solid/cooled state.
/// </summary>
public record AtomOverrideSpec(
    float? RadiusScale = null,
    bool Dimmed = false,
    bool Darkened = false);
