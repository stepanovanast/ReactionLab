namespace ReactionLab.Data.Chemistry;

/// <summary>
/// One phase/step of a reaction, described in chemistry terms.
/// Molecules list defines what's present and in what arrangement.
/// ElectronTransfers drives the particle animation in Three.js.
/// AtomOverrides drives visual changes (size, color) to show ionization or state change.
/// </summary>
public record ReactionStepSpec(
    int Number,
    string Title,
    string Description,
    List<MoleculeSpec> Molecules,
    List<(string From, string To)>? ElectronTransfers = null,
    Dictionary<string, AtomOverrideSpec>? AtomOverrides = null);
