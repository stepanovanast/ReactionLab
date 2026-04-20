using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReactionLab.Data.Chemistry;

/// <summary>
/// Converts a ReactionSpec (chemistry description) into the JSON string
/// stored in the database and consumed by Three.js on the frontend.
///
/// The output format exactly matches the ReactionStep interface in api.service.ts —
/// so the frontend never needs to change regardless of how reactions are defined.
/// </summary>
public static class StepJsonBuilder
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static string Build(ReactionSpec reaction)
    {
        var steps = reaction.Steps.Select(BuildStep).ToList();
        return JsonSerializer.Serialize(steps, JsonOpts);
    }

    private static object BuildStep(ReactionStepSpec step)
    {
        // Compute 3D positions for every molecule in this step
        var allPositions = step.Molecules
            .SelectMany(mol => GeometryEngine.ComputePositions(mol))
            .ToList();

        // Collect all bonds across all molecules
        var allBonds = step.Molecules
            .SelectMany(mol => mol.Bonds)
            .ToList();

        return new
        {
            number = step.Number,
            title = step.Title,
            description = step.Description,
            background = "default",
            atoms = allPositions.Select(a => new
            {
                id = a.Id,
                element = a.Element,
                x = a.X,
                y = a.Y,
                z = a.Z
            }),
            bonds = allBonds.Select(b => new { from = b.From, to = b.To }),
            electronTransfers = (step.ElectronTransfers ?? [])
                .Select(e => new { from = e.From, to = e.To }),
            atomOverrides = BuildOverrides(step.AtomOverrides)
        };
    }

    // Converts the dictionary of overrides into a plain object for JSON serialization
    private static Dictionary<string, object> BuildOverrides(
        Dictionary<string, AtomOverrideSpec>? overrides)
    {
        if (overrides == null) return [];

        return overrides.ToDictionary(
            kvp => kvp.Key,
            kvp => (object)new
            {
                radiusScale = kvp.Value.RadiusScale,
                dimmed      = kvp.Value.Dimmed    ? (bool?)true : null,
                darkened    = kvp.Value.Darkened  ? (bool?)true : null,
            });
    }
}
