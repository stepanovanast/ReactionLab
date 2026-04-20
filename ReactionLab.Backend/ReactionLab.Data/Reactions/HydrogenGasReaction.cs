using ReactionLab.Data.Chemistry;

namespace ReactionLab.Data.Reactions;

/// <summary>
/// Fe + 2HCl → FeCl₂ + H₂  (Production of Hydrogen Gas)
///
/// 4 phases:
///   1. Reactants         — Fe solid separate from 2 HCl molecules
///   2. Acid Dissociation — HCl splits into H⁺ and Cl⁻, ions approach Fe
///   3. Electron Transfer — Fe → Fe²⁺ + 2e⁻; 2H⁺ + 2e⁻ → H₂
///   4. Products          — FeCl₂ ionic compound + H₂ gas bubble
/// </summary>
public static class HydrogenGasReaction
{
    private static readonly AtomSpec Fe1  = new("fe1", "Fe");
    private static readonly AtomSpec H1   = new("h1",  "H");
    private static readonly AtomSpec H2   = new("h2",  "H");
    private static readonly AtomSpec Cl1  = new("cl1", "Cl");
    private static readonly AtomSpec Cl2  = new("cl2", "Cl");

    // Ions (after oxidation/reduction)
    private static readonly AtomSpec Fe1Ion  = Fe1 with { OxidationState =  2 };
    private static readonly AtomSpec H1Ion   = H1  with { OxidationState =  1 };
    private static readonly AtomSpec H2Ion   = H2  with { OxidationState =  1 };
    private static readonly AtomSpec Cl1Ion  = Cl1 with { OxidationState = -1 };
    private static readonly AtomSpec Cl2Ion  = Cl2 with { OxidationState = -1 };

    public static ReactionSpec GetSpec() => new(
    [
        Step1_Reactants(),
        Step2_AcidDissociation(),
        Step3_ElectronTransfer(),
        Step4_Products(),
    ]);

    // -------------------------------------------------------------------------
    // Step 1 — Reactants (room temperature)
    // Fe solid on the left; two HCl molecules on the right, H covalently bonded to Cl.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step1_Reactants() => new(
        Number: 1,
        Title: "Reactants",
        Description: "Iron (Fe) is a solid metal on the left. " +
                     "Two hydrochloric acid (HCl) molecules sit on the right — " +
                     "each is a hydrogen atom covalently bonded to a chlorine atom. " +
                     "The reactants are not yet in contact.",
        Molecules:
        [
            // Fe solid — single atom centred at left
            new MoleculeSpec("metallic_cluster", [Fe1], [],
                CenterX: -2.5f, CenterY: 0f, CenterZ: 0f),

            // HCl molecule 1 (upper)
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX: 1.5f, CenterY: 0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1],
                [new BondSpec("h1", "cl1")],
                CenterX: 2.3f, CenterY: 0.5f, CenterZ: 0f),

            // HCl molecule 2 (lower)
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX: 1.5f, CenterY: -0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2],
                [new BondSpec("h2", "cl2")],
                CenterX: 2.3f, CenterY: -0.5f, CenterZ: 0f),
        ]);

    // -------------------------------------------------------------------------
    // Step 2 — Acid Dissociation
    // HCl ionises in the presence of Fe; H⁺ and Cl⁻ ions float freely around the surface.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step2_AcidDissociation() => new(
        Number: 2,
        Title: "Acid Dissociation",
        Description: "Hydrochloric acid dissociates into H⁺ and Cl⁻ ions. " +
                     "The ions surround the iron surface — the H–Cl covalent bonds are broken. " +
                     "Chloride ions are attracted to the positive iron surface; " +
                     "protons hover nearby, ready to accept electrons.",
        Molecules:
        [
            new MoleculeSpec("metallic_cluster", [Fe1],  [], CenterX:  0.0f, CenterY:  0.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H1Ion], [], CenterX: -1.8f, CenterY:  1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2Ion], [], CenterX: -1.8f, CenterY: -1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1Ion], [], CenterX: 1.8f, CenterY:  0.9f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2Ion], [], CenterX: 1.8f, CenterY: -0.9f, CenterZ: 0f),
        ]);

    // -------------------------------------------------------------------------
    // Step 3 — Electron Transfer
    // Fe donates 2 electrons, one to each H⁺. Fe becomes Fe²⁺; H⁺ atoms are neutralised.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step3_ElectronTransfer() => new(
        Number: 3,
        Title: "Electron Transfer",
        Description: "Iron donates two electrons — one to each hydrogen ion. " +
                     "Fe shrinks as it becomes Fe²⁺ (oxidised). " +
                     "The two H atoms are now neutral and drawn toward each other. " +
                     "The Cl⁻ ions close in on the iron cation.",
        Molecules:
        [
            new MoleculeSpec("metallic_cluster", [Fe1Ion],  [], CenterX:  0.0f, CenterY:  0.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H1],  [], CenterX: -1.8f, CenterY:  0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2],  [], CenterX: -1.8f, CenterY: -0.3f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1Ion], [], CenterX: 1.5f, CenterY:  0.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2Ion], [], CenterX: 1.5f, CenterY: -0.5f, CenterZ: 0f),
        ],
        ElectronTransfers:
        [
            ("fe1", "h1"),
            ("fe1", "h2"),
        ],
        AtomOverrides: new()
        {
            ["fe1"] = new AtomOverrideSpec(RadiusScale: 0.8f, Dimmed: true),
        });

    // -------------------------------------------------------------------------
    // Step 4 — Products
    // FeCl₂ ionic compound forms; H₂ gas molecule rises as a bubble.
    // -------------------------------------------------------------------------
    private static ReactionStepSpec Step4_Products() => new(
        Number: 4,
        Title: "Products Formed",
        Description: "The reaction is complete. Fe²⁺ is flanked by two Cl⁻ ions, " +
                     "forming iron(II) chloride (FeCl₂) dissolved in solution. " +
                     "The two hydrogen atoms bond together as H₂ gas — " +
                     "the colourless bubbles you see rising from the iron.",
        Molecules:
        [
            // FeCl₂: Fe²⁺ flanked by two Cl⁻
            new MoleculeSpec("metallic_cluster", [Fe1Ion], [], CenterX: 1.8f, CenterY: 0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1Ion],
                [new BondSpec("cl1", "fe1")],
                CenterX: 0.5f, CenterY: 0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2Ion],
                [new BondSpec("cl2", "fe1")],
                CenterX: 3.1f, CenterY: 0f, CenterZ: 0f),

            // H₂ gas molecule
            new MoleculeSpec("metallic_cluster", [H1], [], CenterX: -2.0f, CenterY: 0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2],
                [new BondSpec("h1", "h2")],
                CenterX: -1.5f, CenterY: 0f, CenterZ: 0f),
        ],
        AtomOverrides: new()
        {
            ["fe1"] = new AtomOverrideSpec(RadiusScale: 0.8f, Dimmed: true),
        });
}
