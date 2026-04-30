using ReactionLab.Data.Chemistry;

namespace ReactionLab.Data.Reactions;
public static class HydrogenGasReaction
{
    private static readonly AtomSpec Fe1  = new("fe1", "Fe");
    private static readonly AtomSpec H1   = new("h1",  "H");
    private static readonly AtomSpec H2   = new("h2",  "H");
    private static readonly AtomSpec Cl1  = new("cl1", "Cl");
    private static readonly AtomSpec Cl2  = new("cl2", "Cl");
    private static readonly AtomSpec Fe1Ion  = Fe1 with { OxidationState =  2 };
    private static readonly AtomSpec H1Ion   = H1  with { OxidationState =  1 };
    private static readonly AtomSpec H2Ion   = H2  with { OxidationState =  1 };
    private static readonly AtomSpec Cl1Ion  = Cl1 with { OxidationState = -1 };
    private static readonly AtomSpec Cl2Ion  = Cl2 with { OxidationState = -1 };

    public static ReactionSpec GetSpec() => new(
    [
        Step1_InitialCollision(),
        Step2_Displacement(),
        Step3_IronChloride(),
        Step4_ReleasingHydrogenGas(),
    ]);

    // Step 1
    private static ReactionStepSpec Step1_InitialCollision() => new(
        Number: 1,
        Title: "The initial collision",
        Description: "The solid iron atoms come into contact with the hydrochloric acid liquid " +
                     "The acid is corrosive, its molecules are moving fast and hitting the surface of the metal with enough energy to start breaking chemical bonds.",
        Molecules:
        [
            // Fe solid
            new MoleculeSpec("metallic_cluster", [Fe1], [],
                CenterX: -4.0f, CenterY: 0f, CenterZ: 0f),

            // HCl molecule 1
            new MoleculeSpec("metallic_cluster", [H1], [],
                CenterX: 2.5f, CenterY: 1.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1],
                [new BondSpec("h1", "cl1")],
                CenterX: 3.3f, CenterY: 1.5f, CenterZ: 0f),

            // HCl molecule 2
            new MoleculeSpec("metallic_cluster", [H2], [],
                CenterX: 2.5f, CenterY: -1.5f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2],
                [new BondSpec("h2", "cl2")],
                CenterX: 3.3f, CenterY: -1.5f, CenterZ: 0f),
        ]);

    // Step 2
        private static ReactionStepSpec Step2_Displacement() => new(
        Number: 2,
        Title: "The displacement",
        Description: "The iron atoms are more active than the hydrogen atoms in the acid. " +
                     "The iron pushes the hydrogen out of the way so it can take its place. " +
                     "This is called a single replacement reaction because the iron replaces the hydrogen.",
        Molecules:
        [
            new MoleculeSpec("metallic_cluster", [Fe1],  [], CenterX:  0.0f, CenterY:  0.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H1Ion], [], CenterX: -1.8f, CenterY:  1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [H2Ion], [], CenterX: -1.8f, CenterY: -1.0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1Ion], [], CenterX: 1.8f, CenterY:  0.9f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2Ion], [], CenterX: 1.8f, CenterY: -0.9f, CenterZ: 0f),
        ]);

    // Step 3
        private static ReactionStepSpec Step3_IronChloride() => new(
        Number: 3,
        Title: "Formation of iron chloride",
        Description: "Iron donates one electrons to each hydrogen ion. " +
                     "Iron shrinks as it becomes oxidised. " +
                     "The Iron atoms bond with the chlorine atoms to create a new substance called iron chloride. " +
                     "This is a type of salt that dissolves into the liquid, often turning the solution a light green color.",
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

    // Step 4
    
    private static ReactionStepSpec Step4_ReleasingHydrogenGas() => new(
        Number: 4,
        Title: "Releasing hydrogen gas",
        Description: "The hydrogen atoms that were kicked out find each other and pair up to form hydrogen gas. " +
                     "Since gas is lighter than liquid, it forms tiny bubbles that fizz and rise to the surface, escaping into the air.",
        Molecules:
        [
            // FeCl2
            new MoleculeSpec("metallic_cluster", [Fe1Ion], [], CenterX: 1.8f, CenterY: 0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl1Ion],
                [new BondSpec("cl1", "fe1")],
                CenterX: 0.5f, CenterY: 0f, CenterZ: 0f),
            new MoleculeSpec("metallic_cluster", [Cl2Ion],
                [new BondSpec("cl2", "fe1")],
                CenterX: 3.1f, CenterY: 0f, CenterZ: 0f),

            // H2
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
