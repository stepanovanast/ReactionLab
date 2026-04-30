using ReactionLab.Data.Chemistry;

namespace ReactionLab.Data.Reactions;
public static class IronSulfideReaction
{
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
        Title: "Ambient mixture",
        Description: "Iron appears as grey magnetic particles arranged in a metallic " +
                     "grid. Sulfur forms crown rings. " +
                     "The two substances are physically separate; a magnet would still " +
                     "pull the grey particles away.",
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "metallic_cluster",
                Atoms: FeAtoms,
                CenterX: -4.5f),

            new MoleculeSpec(
                GeometryType: "crown_ring",
                Atoms: SAtoms,
                CenterX: 4.5f)
        ]
    );

    // -------------------------------------------------------------------------
    // Step 2 — Activation and Liquefaction (119–445 °C)
    // S₈ ring breaks open. Sulfur becomes molten, surrounding the Fe cluster.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step2_ActivationAndLiquefaction() => new(
        Number: 2,
        Title: "Activation and liquefaction",
        Description: "With heat, the sulfur rings break apart. Sulfur becomes fluid, surrounding " +
                     "and coating the iron particles. The yellow atoms flow around the " +
                     "stationary iron blocks",
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "metallic_cluster",
                Atoms: FeAtoms),

            new MoleculeSpec(
                GeometryType: "free_sphere",
                Atoms: SAtoms,
                Bonds: [])   // explicitly no bonds — S₈ ring has broken
        ]
    );

    // -------------------------------------------------------------------------
    // Step 3 — Exothermic Fusion (600–900 °C)
    // Each Fe atom transfers electrons to a nearby S. Releases ~100 kJ/mol.
    // Fe shrinks (→Fe²⁺), S expands (→S²⁻). Paired transfers for visual clarity.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step3_ExothermicFusion() => new(
        Number: 3,
        Title: "Exothermic fusion",
        Description: "Electrons leap from each iron atom to a sulfur " +
                     "neighbour. Iron shrinks as it becomes oxidised and sulfur expands " +
                     "as it receives electrons. The reaction releases a massive amount of energy.",
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
        Title: "Lattice solidification",
        Description: "The chaos stops. Iron and sulfur ions snap into a rigid crystal " +
                     "and each iron atom is locked between sulfur neighbors above and below. " +
                     "The glow fades into a dull charcoal-black solid",
        Molecules:
        [
            new MoleculeSpec(
                GeometryType: "NiAs_crystal",
                Atoms: [..FeIons, ..SIons])   // 8 Fe²⁺ + 8 S²⁻ — bonds auto-generated by BondRules
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

}
