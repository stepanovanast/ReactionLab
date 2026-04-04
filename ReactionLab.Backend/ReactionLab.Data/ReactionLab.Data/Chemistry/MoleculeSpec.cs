namespace ReactionLab.Data.Chemistry;

/// <summary>
/// Describes a molecule or group of atoms by their chemistry — not their coordinates.
/// GeometryEngine will compute the actual 3D positions based on GeometryType and real bond lengths.
///
/// Supported geometry types:
///   "crown_ring"        — cyclic ring with crown puckering (e.g. S₈)
///   "metallic_cluster"  — square grid of metal atoms (e.g. 2×2 Fe lattice)
///   "free_sphere"       — atoms distributed on a sphere (e.g. molten S around Fe)
///   "NiAs_crystal"      — NiAs-type crystal with metal layer + sulfur above/below (e.g. FeS)
/// </summary>
public record MoleculeSpec(
    string GeometryType,
    List<AtomSpec> Atoms,
    List<BondSpec> Bonds,
    float CenterX = 0f,
    float CenterY = 0f,
    float CenterZ = 0f);
