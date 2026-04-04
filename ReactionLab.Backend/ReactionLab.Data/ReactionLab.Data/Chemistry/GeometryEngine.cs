namespace ReactionLab.Data.Chemistry;

/// <summary>
/// Converts a MoleculeSpec into a list of atoms with computed 3D positions.
/// Positions are calculated from real experimental bond lengths (in Ångströms)
/// then scaled to Three.js units (Scale = 0.6, so 1 unit ≈ 1.67 Å).
///
/// Adding a new geometry type: implement a private static method and add it to the switch.
/// Adding a new element pair: add its bond length to BondLengths.
/// </summary>
public static class GeometryEngine
{
    // 1 Three.js unit = 1 / Scale Ångströms  (0.6 → 1 unit ≈ 1.67 Å)
    private const float Scale = 0.6f;

    // Experimental bond lengths in Ångströms (covalent/metallic radii sums)
    private static readonly Dictionary<string, float> BondLengths = new()
    {
        ["S-S"]   = 2.05f,   // S₈ covalent bond
        ["Fe-Fe"] = 2.48f,   // Fe metallic bond
        ["Fe-S"]  = 2.26f,   // FeS ionic bond
        ["H-H"]   = 0.74f,
        ["O-O"]   = 1.21f,
        ["H-O"]   = 0.96f,
        ["N-N"]   = 1.45f,
        ["C-C"]   = 1.54f,
        ["C-H"]   = 1.09f,
        ["Na-Cl"] = 2.36f,
        ["Cu-Cu"] = 2.56f,
        ["Zn-S"]  = 2.34f,
    };

    public record AtomPosition(string Id, string Element, float X, float Y, float Z);

    /// <summary>
    /// Dispatches to the correct geometry function based on GeometryType.
    /// </summary>
    public static List<AtomPosition> ComputePositions(MoleculeSpec mol) =>
        mol.GeometryType switch
        {
            "crown_ring"       => CrownRing(mol),
            "metallic_cluster" => MetallicCluster(mol),
            "free_sphere"      => FreeSphere(mol),
            "NiAs_crystal"     => NiAsCrystal(mol),
            _ => throw new ArgumentException($"Unknown geometry type: '{mol.GeometryType}'. " +
                 "Supported: crown_ring, metallic_cluster, free_sphere, NiAs_crystal")
        };

    // -------------------------------------------------------------------------
    // CROWN RING  (e.g. S₈)
    // -------------------------------------------------------------------------
    // Atoms sit on a circle of radius r, alternating height ±h.
    // This matches the real D4d crown conformation of S₈.
    // r is derived from the bond length using the inscribed polygon formula,
    // then corrected for puckering.
    private static List<AtomPosition> CrownRing(MoleculeSpec mol)
    {
        int n = mol.Atoms.Count;
        float bondLen = GetBondLength(mol.Atoms[0].Element, mol.Atoms[0].Element) * Scale;

        // Polygon radius: for a flat n-gon, r = bondLen / (2 * sin(π/n))
        // Crown puckering reduces the in-plane radius slightly
        float r = (bondLen / (2f * MathF.Sin(MathF.PI / n))) * 0.95f;
        float h = bondLen * 0.22f; // alternating vertical offset (crown puckering)

        var result = new List<AtomPosition>();
        for (int i = 0; i < n; i++)
        {
            float angle = i * 2f * MathF.PI / n;
            float x = mol.CenterX + r * MathF.Cos(angle);
            float y = mol.CenterY + (i % 2 == 0 ? h : -h);
            float z = mol.CenterZ + r * MathF.Sin(angle);
            result.Add(new(mol.Atoms[i].Id, mol.Atoms[i].Element,
                Round(x), Round(y), Round(z)));
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // METALLIC CLUSTER  (e.g. 8 Fe in a 2×2×2 cube, or n Fe in a flat grid)
    // -------------------------------------------------------------------------
    // 8 atoms → 2×2×2 cube using the metallic bond length as the edge length.
    // Other counts → flat √n × √n square grid (original behaviour).
    private static List<AtomPosition> MetallicCluster(MoleculeSpec mol)
    {
        int n = mol.Atoms.Count;
        float spacing = GetBondLength(mol.Atoms[0].Element, mol.Atoms[0].Element) * Scale;

        // 8 atoms: place in a 2×2×2 cube centred on (CenterX, CenterY, CenterZ)
        if (n == 8)
        {
            float h = spacing / 2f;
            (float dx, float dy, float dz)[] corners =
            [
                (-h, -h, -h), ( h, -h, -h), (-h, -h,  h), ( h, -h,  h),
                (-h,  h, -h), ( h,  h, -h), (-h,  h,  h), ( h,  h,  h),
            ];
            return corners.Select((c, i) => new AtomPosition(
                mol.Atoms[i].Id, mol.Atoms[i].Element,
                Round(mol.CenterX + c.dx),
                Round(mol.CenterY + c.dy),
                Round(mol.CenterZ + c.dz)
            )).ToList();
        }

        // Default: flat square grid (√n × √n)
        int side = (int)MathF.Round(MathF.Sqrt(n));
        float offset = spacing * (side - 1) / 2f;

        var result = new List<AtomPosition>();
        for (int i = 0; i < n; i++)
        {
            int row = i / side;
            int col = i % side;
            float x = mol.CenterX + col * spacing - offset;
            float y = mol.CenterY;
            float z = mol.CenterZ + row * spacing - offset;
            result.Add(new(mol.Atoms[i].Id, mol.Atoms[i].Element,
                Round(x), Round(y), Round(z)));
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // FREE SPHERE  (e.g. molten/free atoms surrounding a central point)
    // -------------------------------------------------------------------------
    // Distributes n atoms on a sphere using the Fibonacci sphere algorithm,
    // which gives a near-uniform distribution regardless of atom count.
    // Radius is computed from the element's own bond length × a spread factor.
    private static List<AtomPosition> FreeSphere(MoleculeSpec mol)
    {
        int n = mol.Atoms.Count;
        float bondLen = GetBondLength(mol.Atoms[0].Element, mol.Atoms[0].Element) * Scale;
        float radius = bondLen * 2.8f; // atoms float at ~2.8× bond distance from center

        var result = new List<AtomPosition>();
        float goldenAngle = MathF.PI * (3f - MathF.Sqrt(5f)); // ~2.399 rad

        for (int i = 0; i < n; i++)
        {
            float t = (float)i / n;
            float inclination = MathF.Acos(1f - 2f * t);
            float azimuth = goldenAngle * i;

            float x = mol.CenterX + radius * MathF.Sin(inclination) * MathF.Cos(azimuth);
            float y = mol.CenterY + radius * MathF.Cos(inclination);
            float z = mol.CenterZ + radius * MathF.Sin(inclination) * MathF.Sin(azimuth);
            result.Add(new(mol.Atoms[i].Id, mol.Atoms[i].Element,
                Round(x), Round(y), Round(z)));
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // NiAs CRYSTAL  (e.g. FeS product)
    // -------------------------------------------------------------------------
    // Two variants:
    //   8 Fe + 8 S  →  layered S–Fe–Fe–S along the Y axis (2×2 grid in XZ each layer)
    //   4 Fe + 8 S  →  original: Fe in 2×2 square, 2 S per Fe above and below
    private static List<AtomPosition> NiAsCrystal(MoleculeSpec mol)
    {
        var feAtoms = mol.Atoms.Where(a => a.Element != "S").ToList();
        var sAtoms  = mol.Atoms.Where(a => a.Element == "S").ToList();

        float feSpacing = GetBondLength("Fe", "Fe") * Scale;
        float feSLen    = GetBondLength("Fe", "S")  * Scale;

        var result = new List<AtomPosition>();

        // --- 8 Fe + 8 S: NiAs-inspired S–Fe–S–Fe stacking along Y ---
        //
        // Lattice constants (FeS NiAs, scaled ×0.6):
        //   a  = 3.44 × 0.6 = 2.064  (Fe–Fe in-plane distance)
        //   c4 = 5.88 × 0.6 / 4 = 0.882  (quarter-cell layer spacing)
        //   h  = a / 2 = 1.032  (half Fe-square side)
        //
        // S atoms sit at edge-midpoints of the Fe square → Fe–S distance
        //   = √(h² + c4²) = √(1.065 + 0.778) ≈ 1.357 ≈ feSLen ✓
        //
        // Layer stack (centred on mol.CenterY):
        //   y = −3c4/2  →  s1–s4  (outer S, below bottom Fe)
        //   y = −c4/2   →  fe1–fe4 (bonds to s1–s4 below + s5–s8 above → 4-fold)
        //   y = +c4/2   →  s5–s8  (shared S between the two Fe layers)
        //   y = +3c4/2  →  fe5–fe8 (bonds to s5–s8 below only → 2-fold, surface)
        if (feAtoms.Count == 8 && sAtoms.Count == 8)
        {
            const float a  = 2.064f;
            const float c4 = 0.882f;
            float h = a / 2f; // = 1.032

            // Fe at square corners, S at square edge-midpoints (in XZ plane)
            (float x, float z)[] feXZ = [ (-h, -h), ( h, -h), (-h,  h), ( h,  h) ];
            (float x, float z)[] sXZ  = [ ( 0, -h), ( h,  0), ( 0,  h), (-h,  0) ];

            float cy = mol.CenterY;
            float cx = mol.CenterX;
            float cz = mol.CenterZ;

            // s1–s4 (outer bottom S)
            for (int i = 0; i < 4; i++)
                result.Add(new(sAtoms[i].Id, sAtoms[i].Element,
                    Round(cx + sXZ[i].x), Round(cy - 1.5f * c4), Round(cz + sXZ[i].z)));

            // fe1–fe4 (bottom Fe, 4-fold)
            for (int i = 0; i < 4; i++)
                result.Add(new(feAtoms[i].Id, feAtoms[i].Element,
                    Round(cx + feXZ[i].x), Round(cy - 0.5f * c4), Round(cz + feXZ[i].z)));

            // s5–s8 (shared middle S)
            for (int i = 0; i < 4; i++)
                result.Add(new(sAtoms[4 + i].Id, sAtoms[4 + i].Element,
                    Round(cx + sXZ[i].x), Round(cy + 0.5f * c4), Round(cz + sXZ[i].z)));

            // fe5–fe8 (top Fe, 2-fold surface)
            for (int i = 0; i < 4; i++)
                result.Add(new(feAtoms[4 + i].Id, feAtoms[4 + i].Element,
                    Round(cx + feXZ[i].x), Round(cy + 1.5f * c4), Round(cz + feXZ[i].z)));

            return result;
        }

        // --- 4 Fe + 8 S: original — 2 S per Fe, one above, one below ---
        int side   = (int)MathF.Round(MathF.Sqrt(feAtoms.Count));
        float offset = feSpacing * (side - 1) / 2f;

        var fePositions = new Dictionary<string, (float x, float z)>();

        for (int i = 0; i < feAtoms.Count; i++)
        {
            int row = i / side;
            int col = i % side;
            float x = mol.CenterX + col * feSpacing - offset;
            float z = mol.CenterZ + row * feSpacing - offset;
            fePositions[feAtoms[i].Id] = (x, z);
            result.Add(new(feAtoms[i].Id, feAtoms[i].Element, Round(x), mol.CenterY, Round(z)));
        }

        int sIdx = 0;
        foreach (var (_, (fx, fz)) in fePositions)
        {
            if (sIdx < sAtoms.Count)
            {
                result.Add(new(sAtoms[sIdx].Id, sAtoms[sIdx].Element,
                    Round(fx), Round(mol.CenterY + feSLen), Round(fz)));
                sIdx++;
            }
            if (sIdx < sAtoms.Count)
            {
                result.Add(new(sAtoms[sIdx].Id, sAtoms[sIdx].Element,
                    Round(fx), Round(mol.CenterY - feSLen), Round(fz)));
                sIdx++;
            }
        }

        return result;
    }

    // -------------------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------------------

    private static float GetBondLength(string el1, string el2)
    {
        var key1 = $"{el1}-{el2}";
        var key2 = $"{el2}-{el1}";
        if (BondLengths.TryGetValue(key1, out float v)) return v;
        if (BondLengths.TryGetValue(key2, out float v2)) return v2;
        // Fallback: sum of approximate atomic radii in Å
        return (AtomicRadius(el1) + AtomicRadius(el2));
    }

    private static float AtomicRadius(string element) => element switch
    {
        "H"  => 0.31f, "C"  => 0.77f, "N"  => 0.75f, "O"  => 0.73f,
        "S"  => 1.03f, "Fe" => 1.26f, "Cu" => 1.28f, "Zn" => 1.22f,
        "Na" => 1.86f, "Cl" => 0.99f,
        _ => 1.20f  // generic fallback
    };

    private static float Round(float v) => MathF.Round(v * 100f) / 100f;
}
