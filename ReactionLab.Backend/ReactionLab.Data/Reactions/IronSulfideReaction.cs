using ReactionLab.Data.Chemistry;

namespace ReactionLab.Data.Reactions;

/// <summary>
/// Fe + S → FeS  (Iron Sulfide Formation)
///
/// 4 phases:
///   1. Ambient Mixture       — Fe metallic lattice + S₈ crown ring, physically separate
///   2. Activation            — S₈ ring breaks, molten S atoms surround Fe cluster
///   3. Exothermic Fusion     — electron transfer Fe→S, both ions form
///   4. Lattice Solidification — Fe²⁺ and S²⁻ lock into NiAs-type crystal
/// </summary>
public static class IronSulfideReaction
{
    // Atom IDs used consistently across all steps so Three.js can tween positions
    private static readonly List<AtomSpec> FeAtoms =
    [
        new("fe1", "Fe"), new("fe2", "Fe"), new("fe3", "Fe"), new("fe4", "Fe"),
        new("fe5", "Fe"), new("fe6", "Fe"), new("fe7", "Fe"), new("fe8", "Fe"),
    ];

    private static readonly List<AtomSpec> SAtoms =
    [
        new("s1","S"), new("s2","S"), new("s3","S"), new("s4","S"),
        new("s5","S"), new("s6","S"), new("s7","S"), new("s8","S")
    ];

    // Fe²⁺ (lost electrons → smaller)
    private static readonly List<AtomSpec> FeIons =
        FeAtoms.Select(a => a with { OxidationState = 2 }).ToList();

    // S²⁻ (gained electrons → larger)
    private static readonly List<AtomSpec> SIons =
        SAtoms.Select(a => a with { OxidationState = -2 }).ToList();

    public static ReactionSpec GetSpec() => new(
    [
        Step1_AmbientMixture(),
        Step2_ActivationAndLiquefaction(),
        Step3_ExothermicFusion(),
        Step4_LatticeSolidification(),
    ]);

    // -------------------------------------------------------------------------
    // Step 1 — Ambient Mixture (20–115 °C)
    // Fe lattice on the left, S₈ crown ring on the right. No interaction yet.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step1_AmbientMixture() => new(
        Number: 1,
        Title: "Ambient Mixture",
        Description: "Iron (Fe) appears as grey magnetic particles arranged in a metallic " +
                     "lattice. Sulfur (S) forms S₈ crown rings — 8 yellow atoms bonded in a " +
                     "circle. The two substances are physically separate; a magnet would still " +
                     "pull the grey particles away.",
        TempStart: 20, TempEnd: 115,
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "metallic_cluster",
                Atoms: FeAtoms,
                Bonds: FeMetallicBonds(),
                CenterX: -2.8f),

            new MoleculeSpec(
                GeometryType: "crown_ring",
                Atoms: SAtoms,
                Bonds: S8Bonds(),
                CenterX: 2.8f)
        ]
    );

    // -------------------------------------------------------------------------
    // Step 2 — Activation and Liquefaction (119–445 °C)
    // S₈ ring breaks open. Sulfur becomes molten, surrounding the Fe cluster.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step2_ActivationAndLiquefaction() => new(
        Number: 2,
        Title: "Activation and Liquefaction",
        Description: "At 119 °C the S₈ rings break apart. Sulfur becomes fluid, surrounding " +
                     "and coating the iron particles. The yellow atoms flow around the " +
                     "stationary iron blocks — the wetting phase.",
        TempStart: 119, TempEnd: 445,
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "metallic_cluster",
                Atoms: FeAtoms,
                Bonds: FeMetallicBonds()),

            new MoleculeSpec(
                GeometryType: "free_sphere",
                Atoms: SAtoms,
                Bonds: [])   // no bonds — S₈ ring has broken
        ]
    );

    // -------------------------------------------------------------------------
    // Step 3 — Exothermic Fusion (600–900 °C)
    // Each Fe atom transfers electrons to a nearby S. Releases ~100 kJ/mol.
    // Fe shrinks (→Fe²⁺), S expands (→S²⁻). Paired transfers for visual clarity.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step3_ExothermicFusion() => new(
        Number: 3,
        Title: "Exothermic Fusion",
        Description: "The high-energy moment: electrons leap from each iron atom to a sulfur " +
                     "neighbour. Iron shrinks as it becomes Fe²⁺ and sulfur expands as it " +
                     "becomes S²⁻. The reaction releases ~100 kJ/mol.",
        TempStart: 600, TempEnd: 900,
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "metallic_cluster",
                Atoms: FeIons,   // Fe²⁺ oxidation state
                Bonds: []),

            new MoleculeSpec(
                GeometryType: "free_sphere",
                Atoms: SIons,    // S²⁻ oxidation state
                Bonds: [])
        ],
        // All 8 Fe atoms each transfer 2 electrons to the nearest S neighbour.
        ElectronTransfers:
        [
            ("fe1", "s1"), ("fe2", "s2"), ("fe3", "s3"), ("fe4", "s4"),
            ("fe5", "s5"), ("fe6", "s6"), ("fe7", "s7"), ("fe8", "s8"),
        ],
        AtomOverrides: new()
        {
            // Fe²⁺: smaller (lost 2 electrons), greyed out
            ["fe1"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe2"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe3"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe4"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe5"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe6"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe7"] = new(RadiusScale: 0.8f, Dimmed: true),
            ["fe8"] = new(RadiusScale: 0.8f, Dimmed: true),
            // S²⁻: larger (gained 2 electrons)
            ["s1"]  = new(RadiusScale: 1.2f),
            ["s2"]  = new(RadiusScale: 1.2f),
            ["s3"]  = new(RadiusScale: 1.2f),
            ["s4"]  = new(RadiusScale: 1.2f),
            ["s5"]  = new(RadiusScale: 1.2f),
            ["s6"]  = new(RadiusScale: 1.2f),
            ["s7"]  = new(RadiusScale: 1.2f),
            ["s8"]  = new(RadiusScale: 1.2f),
        }
    );

    // -------------------------------------------------------------------------
    // Step 4 — Lattice Solidification (900 °C cooling to 25 °C)
    // Fe²⁺ and S²⁻ snap into a rigid NiAs-type crystal. Black, non-magnetic solid.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step4_LatticeSolidification() => new(
        Number: 4,
        Title: "Lattice Solidification",
        Description: "The chaos stops. Fe²⁺ and S²⁻ ions snap into a rigid NiAs-type crystal " +
                     "lattice — each iron atom locked between sulfur neighbours above and below. " +
                     "The glow fades into a dull charcoal-black solid: non-magnetic, brittle, " +
                     "and complete.",
        TempStart: 900, TempEnd: 25,
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "NiAs_crystal",
                Atoms: [..FeIons, ..SIons],   // 4 Fe²⁺ + 8 S²⁻ in one crystal
                Bonds: NiAsBonds())
        ],
        AtomOverrides: new()
        {
            ["fe1"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe2"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe3"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe4"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe5"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe6"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe7"] = new(RadiusScale: 0.8f, Darkened: true),
            ["fe8"] = new(RadiusScale: 0.8f, Darkened: true),
            ["s1"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s2"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s3"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s4"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s5"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s6"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s7"]  = new(RadiusScale: 1.2f, Darkened: true),
            ["s8"]  = new(RadiusScale: 1.2f, Darkened: true),
        }
    );

    // -------------------------------------------------------------------------
    // Bond helpers
    // -------------------------------------------------------------------------

    // Fe 2×2×2 metallic cube: 12 edges of the cube.
    // Bottom layer: fe1(-,-,-) fe2(+,-,-) fe3(-,-,+) fe4(+,-,+)
    // Top layer:    fe5(-,+,-) fe6(+,+,-) fe7(-,+,+) fe8(+,+,+)
    // These bonds break in step 3 when the metallic lattice disrupts during ionisation.
    private static List<BondSpec> FeMetallicBonds() =>
    [
        // Bottom face
        new("fe1", "fe2", "metallic"), new("fe3", "fe4", "metallic"),
        new("fe1", "fe3", "metallic"), new("fe2", "fe4", "metallic"),
        // Top face
        new("fe5", "fe6", "metallic"), new("fe7", "fe8", "metallic"),
        new("fe5", "fe7", "metallic"), new("fe6", "fe8", "metallic"),
        // Vertical pillars
        new("fe1", "fe5", "metallic"), new("fe2", "fe6", "metallic"),
        new("fe3", "fe7", "metallic"), new("fe4", "fe8", "metallic"),
    ];

    // S₈ crown ring: each atom bonds to the next, last bonds back to first
    private static List<BondSpec> S8Bonds() =>
    [
        new("s1","s2"), new("s2","s3"), new("s3","s4"), new("s4","s5"),
        new("s5","s6"), new("s6","s7"), new("s7","s8"), new("s8","s1"),
    ];

    // NiAs crystal bonds — S atoms sit at edge-midpoints of the Fe square.
    // Each bottom Fe (fe1–fe4) bonds to its 2 nearest S in the outer layer (s1–s4)
    // AND its 2 nearest S in the shared middle layer (s5–s8) → 4-fold coordination.
    // Each top Fe (fe5–fe8) bonds to its 2 nearest S in the shared middle layer → 2-fold (surface).
    //
    // XZ layout:  feXZ corners (-h,-h),(+h,-h),(-h,+h),(+h,+h)
    //             sXZ  edges   (0,-h),(+h,0),(0,+h),(-h,0)
    //             → fe1(SW) nearest: s1(S-edge), s4(W-edge)
    //               fe2(SE) nearest: s1(S-edge), s2(E-edge)
    //               fe3(NW) nearest: s3(N-edge), s4(W-edge)
    //               fe4(NE) nearest: s2(E-edge), s3(N-edge)
    private static List<BondSpec> NiAsBonds() =>
    [
        // fe1–fe4 → outer S (s1–s4) below
        new("fe1","s1","ionic"), new("fe1","s4","ionic"),
        new("fe2","s1","ionic"), new("fe2","s2","ionic"),
        new("fe3","s3","ionic"), new("fe3","s4","ionic"),
        new("fe4","s2","ionic"), new("fe4","s3","ionic"),
        // fe1–fe4 → shared S (s5–s8) above
        new("fe1","s5","ionic"), new("fe1","s8","ionic"),
        new("fe2","s5","ionic"), new("fe2","s6","ionic"),
        new("fe3","s7","ionic"), new("fe3","s8","ionic"),
        new("fe4","s6","ionic"), new("fe4","s7","ionic"),
        // fe5–fe8 → shared S (s5–s8) below
        new("fe5","s5","ionic"), new("fe5","s8","ionic"),
        new("fe6","s5","ionic"), new("fe6","s6","ionic"),
        new("fe7","s7","ionic"), new("fe7","s8","ionic"),
        new("fe8","s6","ionic"), new("fe8","s7","ionic"),
    ];
}
