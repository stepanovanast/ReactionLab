namespace ReactionLab.Data.Chemistry;

/// <summary>
/// A single atom — its ID used for referencing in bonds/transfers,
/// its chemical symbol, and its oxidation state (0 = neutral, +2 = Fe²⁺, -2 = S²⁻).
/// </summary>
public record AtomSpec(string Id, string Element, int OxidationState = 0);

/// <summary>
/// A bond between two atoms by ID.
/// Type: "covalent", "ionic", or "metallic" — informational, used for future rendering hints.
/// </summary>
public record BondSpec(string From, string To, string Type = "covalent");

/// <summary>
/// Visual overrides for a specific atom in a step.
/// RadiusScale: shrink/grow to show ionisation (Fe²⁺ shrinks, S²⁻ expands).
/// Dimmed: atom has lost electrons — rendered greyed out.
/// Darkened: atom is in a solid/cooled state.
/// </summary>
public record AtomOverrideSpec(
    float? RadiusScale = null,
    bool Dimmed = false,
    bool Darkened = false);

/// <summary>
/// Describes a molecule or group of atoms by chemistry — not coordinates.
/// GeometryEngine computes 3D positions from GeometryType and real bond lengths.
///
/// Supported geometry types:
///   "crown_ring"        — cyclic ring with crown puckering (e.g. S₈)
///   "metallic_cluster"  — square grid or cube of metal atoms (e.g. Fe lattice)
///   "free_sphere"       — atoms distributed on a sphere (e.g. molten S around Fe)
///   "NiAs_crystal"      — NiAs-type crystal with metal + sulfur layers (e.g. FeS)
///
/// Bonds sentinel values:
///   null   → BondRules auto-generates bonds from the geometry type (default)
///   []     → explicitly no bonds (e.g. broken ring, free ions after dissociation)
///   [...]  → use exactly these bonds (complex or cross-molecule connections)
/// </summary>
public record MoleculeSpec(
    string GeometryType,
    List<AtomSpec> Atoms,
    List<BondSpec>? Bonds = null,
    float CenterX = 0f,
    float CenterY = 0f,
    float CenterZ = 0f);

/// <summary>
/// One phase/step of a reaction.
/// Molecules defines what's present and in what arrangement.
/// ElectronTransfers drives the particle animation in Three.js.
/// AtomOverrides drives visual changes (size, colour) to show ionisation or state change.
/// </summary>
public record ReactionStepSpec(
    int Number,
    string Title,
    string Description,
    List<MoleculeSpec> Molecules,
    List<(string From, string To)>? ElectronTransfers = null,
    Dictionary<string, AtomOverrideSpec>? AtomOverrides = null);

/// <summary>
/// The full chemistry definition for a reaction — just its ordered steps.
/// Pass this to StepJsonBuilder.Build() to get the JSON string for the database.
/// </summary>
public record ReactionSpec(List<ReactionStepSpec> Steps);
