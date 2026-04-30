using ReactionLab.Data.Chemistry;

namespace ReactionLab.Data.Reactions;

/// <summary>
/// 2H₂O₂ → 2H₂O + O₂  (Glowing Splint Test / H₂O₂ Decomposition)
///
/// 4 phases:
///   1. Reactants         — two H₂O₂ molecules, each with a central O–O peroxide bond
///   2. O–O Cleavage      — catalyst breaks the peroxide bonds; four OH radicals form
///   3. Rearrangement     — OH radicals recombine; two H₂O molecules take shape; lone O atoms approach
///   4. Products          — two H₂O molecules + O₂ gas (the oxygen that relights the glowing splint)
/// </summary>
public static class GlowingSplintReaction
{
    private static readonly AtomSpec O1 = new("o1", "O");
    private static readonly AtomSpec O2 = new("o2", "O");
    private static readonly AtomSpec O3 = new("o3", "O");
    private static readonly AtomSpec O4 = new("o4", "O");
    private static readonly AtomSpec H1 = new("h1", "H");
    private static readonly AtomSpec H2 = new("h2", "H");
    private static readonly AtomSpec H3 = new("h3", "H");
    private static readonly AtomSpec H4 = new("h4", "H");

    public static ReactionSpec GetSpec() => new(
    [
        Step1_StartingLiquid(),
        Step2_Breakup(),
        Step3_GasAndWater(),
        Step4_GasRelease(),
    ]);

    // -------------------------------------------------------------------------
    // Step 1 — Reactants
    // Two H₂O₂ molecules. Each has a weak O–O peroxide bond in the middle.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step1_StartingLiquid() => new(
        Number: 1,
        Title: "The starting liquid",
        Description: "You begin with hydrogen peroxide, which is unstable. " +
                     "Its atoms are loosely held together and are ready to rearrange into something more stable. ",


                     
        Molecules:
        [
            // H₂O₂ molecule 1 (upper row)
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX: -2.5f, CenterY: 0.7f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1], [],
                CenterX: -1.7f, CenterY: 0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O2],
                [new BondSpec("h1","o1"), new BondSpec("o1","o2"), new BondSpec("o2","h2")],
                CenterX: -0.9f, CenterY: 0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX: -0.1f, CenterY: 0.7f, CenterZ: 0f),

            // H₂O₂ molecule 2 (lower row)
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX: -2.5f, CenterY: -0.7f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O3], [],
                CenterX: -1.7f, CenterY: -0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O4],
                [new BondSpec("h3","o3"), new BondSpec("o3","o4"), new BondSpec("o4","h4")],
                CenterX: -0.9f, CenterY: -0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX: -0.1f, CenterY: -0.7f, CenterZ: 0f),
        ]);

    // -------------------------------------------------------------------------
    // Step 2 — O–O Bond Cleavage
    // The catalyst breaks the peroxide bonds. Four OH radicals are released.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step2_Breakup() => new(
        Number: 2,
        Title: "The breakup",
        Description: "The chemical bonds in the hydrogen peroxide snap apart. " +
                     "This usually happens faster if you add a catalyst (a helper substance) or heat. " +
                     "Once the bonds break, the hydrogen and oxygen atoms are free to find new partners.",
        Molecules:
        [
            // OH radicals from H₂O₂ #1
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX: -2.2f, CenterY: 1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1],
                [new BondSpec("h1","o1")],
                CenterX: -1.5f, CenterY: 0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O2],
                [new BondSpec("o2","h2")],
                CenterX: 0.0f, CenterY: 0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX: 0.7f, CenterY: 1.0f, CenterZ: 0f),

            // OH radicals from H₂O₂ #2
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX: -2.2f, CenterY: -1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O3],
                [new BondSpec("h3","o3")],
                CenterX: -1.5f, CenterY: -0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O4],
                [new BondSpec("o4","h4")],
                CenterX: 0.0f, CenterY: -0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX: 0.7f, CenterY: -1.0f, CenterZ: 0f),
        ]);

    // -------------------------------------------------------------------------
    // Step 3 — Rearrangement
    // Each pair of OH radicals recombines into H₂O. The lone O atoms approach each other.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step3_GasAndWater() => new(
        Number: 3,
        Title: "Rearrangement",
        Description: "As the atoms rearrange, most of them form liquid water.  " +
                     "Each oxygen atom picks up two hydrogen atoms to form a water molecule. " +
                     "The two remaining oxygen atoms are drawn toward each other, " +
                     "beginning to form the O₂ molecule.",
        Molecules:
        [
            // H₂O #1 forming (o1 + h1 + h2)
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX: -2.1f, CenterY: 0.8f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1],
                [new BondSpec("h1","o1"), new BondSpec("h2","o1")],
                CenterX: -1.5f, CenterY: 0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX: -0.9f, CenterY: 0.8f, CenterZ: 0f),

            // H₂O #2 forming (o3 + h3 + h4)
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX: -2.1f, CenterY: -0.8f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O3],
                [new BondSpec("h3","o3"), new BondSpec("h4","o3")],
                CenterX: -1.5f, CenterY: -0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX: -0.9f, CenterY: -0.8f, CenterZ: 0f),

            // O atoms approaching each other to form O₂
            new MoleculeSpec("metallic_cluster", [O2], [],
                CenterX: 1.3f, CenterY: 0.2f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O4], [],
                CenterX: 1.3f, CenterY: -0.2f, CenterZ: 0f),
        ]);

    // -------------------------------------------------------------------------
    // Step 4 — Products
    // Two water molecules and one O₂ gas molecule. The O₂ relights a glowing splint.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step4_GasRelease() => new(
        Number: 4,
        Title: "Releasing oxygen gas",
        Description: "The reaction is complete. Two water molecules (H₂O) are formed, " +
                     "and the two lone oxygen atoms bond together as O₂ gas. " +
                     "Instead of staying in the liquid, this gas bubbles up and fills the empty space in the tube. " +
                     "This high concentration of oxygen is what provides the fuel to relight the glowing splint.",
        Molecules:
        [
            // H₂O molecule 1
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX: -2.4f, CenterY: 0.7f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O1],
                [new BondSpec("h1","o1"), new BondSpec("h2","o1")],
                CenterX: -1.7f, CenterY: 0.2f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX: -1.0f, CenterY: 0.7f, CenterZ: 0f),

            // H₂O molecule 2
            new MoleculeSpec("metallic_cluster", [H3], [],
                CenterX: -2.4f, CenterY: -0.7f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O3],
                [new BondSpec("h3","o3"), new BondSpec("h4","o3")],
                CenterX: -1.7f, CenterY: -0.2f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H4], [],
                CenterX: -1.0f, CenterY: -0.7f, CenterZ: 0f),

            // O₂ gas
            new MoleculeSpec("metallic_cluster", [O2], [],
                CenterX: 1.5f, CenterY: 0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [O4],
                [new BondSpec("o2","o4")],
                CenterX: 2.2f, CenterY: 0f, CenterZ: 0f),
        ]);
}
