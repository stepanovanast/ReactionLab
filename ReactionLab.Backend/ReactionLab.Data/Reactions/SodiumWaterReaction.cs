using ReactionLab.Data.Chemistry;

namespace ReactionLab.Data.Reactions;
public static class SodiumWaterReaction
{
    private static readonly AtomSpec Na1 = new("na1", "Na");
    private static readonly AtomSpec Na2 = new("na2", "Na");
    private static readonly AtomSpec O1  = new("o1",  "O");
    private static readonly AtomSpec O2  = new("o2",  "O");
    private static readonly AtomSpec H1  = new("h1",  "H");  // leaves O1 as H•
    private static readonly AtomSpec H2  = new("h2",  "H");  // stays on O1 → NaOH
    private static readonly AtomSpec H3  = new("h3",  "H");  // leaves O2 as H•
    private static readonly AtomSpec H4  = new("h4",  "H");  // stays on O2 → NaOH

    // Ions formed after electron transfer
    private static readonly AtomSpec Na1Ion = Na1 with { OxidationState =  1 };
    private static readonly AtomSpec Na2Ion = Na2 with { OxidationState =  1 };
    private static readonly AtomSpec O1Ion  = O1  with { OxidationState = -2 };
    private static readonly AtomSpec O2Ion  = O2  with { OxidationState = -2 };

    public static ReactionSpec GetSpec() => new(
    [
        Step1_SurfaceContact(),
        Step2_Tearing(),
        Step3_CreatingNaOH(),
        Step4_HydrogenAndEnergy(),
    ]);

    // Step 1
        private static ReactionStepSpec Step1_SurfaceContact() => new(
        Number: 1,
        Title: "The surface contact",
        Description: "When the solid sodium hits the water, it doesn't just sink. " +
                     "It is less dense than water, so it floats on the surface. " +
                     "Because sodium is so reactive, the chemical reaction starts the millisecond they touch.",
        Molecules:
        [
            // Na atom 1 (upper left)
            new MoleculeSpec("metallic_cluster", [Na1], [],
                CenterX: -2.5f, CenterY:  0.6f, CenterZ: 0f),

            // Na atom 2 (lower left)
            new MoleculeSpec("metallic_cluster", [Na2], [],
                CenterX: -2.5f, CenterY: -0.6f, CenterZ: 0f),

            // H₂O molecule 1 (upper right) — H1–O1–H2
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX:  1.2f, CenterY:  1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1],
                [new BondSpec("h1","o1"), new BondSpec("h2","o1")],
                CenterX:  2.0f, CenterY:  0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX:  2.8f, CenterY:  1.0f, CenterZ: 0f),

            // H₂O molecule 2 (lower right) — H3–O2–H4
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX:  1.2f, CenterY: -1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O2],
                [new BondSpec("h3","o2"), new BondSpec("h4","o2")],
                CenterX:  2.0f, CenterY: -0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX:  2.8f, CenterY: -1.0f, CenterZ: 0f),
        ]);

    // Step 2
    private static ReactionStepSpec Step2_Tearing() => new(
        Number: 2,
        Title: "Tearing water apart",
        Description: "The sodium atoms are aggressive and need to lose an electron to become stable. " +
                     "They force their way into the water molecules, snapping the bonds between the oxygen and hydrogen. " + 
                     "This releases a huge amount of heat energy, often melting the sodium.",

        Molecules:
        [
            // Na1 moved closer to O1
            new MoleculeSpec("metallic_cluster", [Na1], [],
                CenterX: -0.4f, CenterY:  0.6f, CenterZ: 0f),

            // Na2 moved closer to O2
            new MoleculeSpec("metallic_cluster", [Na2], [],
                CenterX: -0.4f, CenterY: -0.6f, CenterZ: 0f),

            // H₂O #1 — H1 starting to drift away
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX:  0.8f, CenterY:  1.2f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1],
                [new BondSpec("h1","o1"), new BondSpec("h2","o1")],
                CenterX:  1.6f, CenterY:  0.4f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX:  2.4f, CenterY:  0.9f, CenterZ: 0f),

            // H₂O #2 — H3 starting to drift away
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX:  0.8f, CenterY: -1.2f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O2],
                [new BondSpec("h3","o2"), new BondSpec("h4","o2")],
                CenterX:  1.6f, CenterY: -0.4f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX:  2.4f, CenterY: -0.9f, CenterZ: 0f),
        ]);

    // Step 3
    private static ReactionStepSpec Step3_CreatingNaOH() => new(
        Number: 3,
        Title: "Creating sodium hydroxide",
        Description: "Each sodium atom donates its single electron to a water molecule. " +
                     "Sodium becomes oxidised. " +
                     "This is a strong base that dissolves instantly into the water. " +
                     "If you added a special indicator liquid, the water would change color to show a base is now present.",
        Molecules:
        [
            // Na1⁺ near O1
            new MoleculeSpec("metallic_cluster", [Na1Ion], [],
                CenterX: -0.2f, CenterY:  0.4f, CenterZ: 0f),

            // Na2⁺ near O2
            new MoleculeSpec("metallic_cluster", [Na2Ion], [],
                CenterX: -0.2f, CenterY: -0.4f, CenterZ: 0f),

            // O1 with H2 still attached; H1 freed
            new MoleculeSpec("metallic_cluster", [O1Ion],
                [new BondSpec("h2","o1")],
                CenterX:  1.4f, CenterY:  0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX:  2.2f, CenterY:  0.7f, CenterZ: 0f),

            // O2 with H4 still attached; H3 freed
            new MoleculeSpec("metallic_cluster", [O2Ion],
                [new BondSpec("h4","o2")],
                CenterX:  1.4f, CenterY: -0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX:  2.2f, CenterY: -0.7f, CenterZ: 0f),

            // H1 and H3 freed — approaching each other
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX:  3.0f, CenterY:  0.2f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX:  3.0f, CenterY: -0.2f, CenterZ: 0f),
        ],
        ElectronTransfers:
        [
            ("na1", "o1"),
            ("na2", "o2"),
        ],
        AtomOverrides: new()
        {
            ["na1"] = new AtomOverrideSpec(RadiusScale: 0.7f, Dimmed: true),
            ["na2"] = new AtomOverrideSpec(RadiusScale: 0.7f, Dimmed: true),
        });

    // Step 4
    
    private static ReactionStepSpec Step4_HydrogenAndEnergy() => new(
        Number: 4,
        Title: "Hydrogen gas and energy",
        Description: "The reaction is complete. " +
                     "Two sodium hydroxide (NaOH) units are formed in solution. " +
                     "The two freed hydrogen atoms bond together as a gas, " +
                     "producing the energetic fizzing and popping you see when sodium " +
                     "is dropped into water.",
        Molecules:
        [
            // NaOH #1 — Na1⁺ bonded to O1–H2
            new MoleculeSpec("metallic_cluster", [Na1Ion], [],
                CenterX: -1.8f, CenterY:  0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1Ion],
                [new BondSpec("na1","o1"), new BondSpec("h2","o1")],
                CenterX: -0.7f, CenterY:  0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX:  0.1f, CenterY:  0.9f, CenterZ: 0f),

            // NaOH #2 — Na2⁺ bonded to O2–H4
            new MoleculeSpec("metallic_cluster", [Na2Ion], [],
                CenterX: -1.8f, CenterY: -0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O2Ion],
                [new BondSpec("na2","o2"), new BondSpec("h4","o2")],
                CenterX: -0.7f, CenterY: -0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX:  0.1f, CenterY: -0.9f, CenterZ: 0f),

            // H₂ gas
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX:  2.2f, CenterY:  0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H3],
                [new BondSpec("h1","h3")],
                CenterX:  2.9f, CenterY:  0f, CenterZ: 0f),
        ],
        AtomOverrides: new()
        {
            ["na1"] = new AtomOverrideSpec(RadiusScale: 0.7f, Dimmed: true),
            ["na2"] = new AtomOverrideSpec(RadiusScale: 0.7f, Dimmed: true),
        });
}
