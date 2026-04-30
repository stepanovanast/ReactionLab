namespace ReactionLab.Data.Chemistry;

/// <summary>
/// Auto-generates bonds for a molecule based on its geometry type and atom list.
/// Called by StepJsonBuilder when MoleculeSpec.Bonds is null.
///
/// Sentinel values on MoleculeSpec.Bonds:
///   null   → run these rules (geometry-driven auto-bonds)
///   []     → explicitly no bonds (e.g. broken ring, free ions)
///   [...]  → use exactly the bonds specified (complex or cross-molecule bonds)
/// </summary>
public static class BondRules
{
    public static List<BondSpec> Generate(MoleculeSpec mol) =>
        mol.GeometryType switch
        {
            "crown_ring"       => CrownRing(mol.Atoms),
            "metallic_cluster" => MetallicCluster(mol.Atoms),
            "NiAs_crystal"     => NiAsCrystal(mol.Atoms),
            "free_sphere"      => [],
            _                  => []
        };

    // Sequential ring: each atom bonds to the next, last wraps back to first.
    private static List<BondSpec> CrownRing(List<AtomSpec> atoms) =>
        Enumerable.Range(0, atoms.Count)
            .Select(i => new BondSpec(atoms[i].Id, atoms[(i + 1) % atoms.Count].Id))
            .ToList();

    private static List<BondSpec> MetallicCluster(List<AtomSpec> atoms)
    {
        if (atoms.Count <= 1) return [];

        // 2×2×2 cube — 12 edges. Corner order matches GeometryEngine.MetallicCluster:
        //   0(-h,-h,-h) 1(+h,-h,-h) 2(-h,-h,+h) 3(+h,-h,+h)
        //   4(-h,+h,-h) 5(+h,+h,-h) 6(-h,+h,+h) 7(+h,+h,+h)
        if (atoms.Count == 8)
        {
            int[][] edges = [
                [0,1],[2,3],[0,2],[1,3],   // bottom face
                [4,5],[6,7],[4,6],[5,7],   // top face
                [0,4],[1,5],[2,6],[3,7],   // vertical pillars
            ];
            return edges.Select(e => new BondSpec(atoms[e[0]].Id, atoms[e[1]].Id, "metallic")).ToList();
        }

        // Flat √n×√n grid: bond right and bond down.
        int side = (int)MathF.Round(MathF.Sqrt(atoms.Count));
        var bonds = new List<BondSpec>();
        for (int i = 0; i < atoms.Count; i++)
        {
            int row = i / side, col = i % side;
            if (col + 1 < side) bonds.Add(new(atoms[i].Id, atoms[i + 1].Id, "metallic"));
            if (row + 1 < side) bonds.Add(new(atoms[i].Id, atoms[i + side].Id, "metallic"));
        }
        return bonds;
    }

    // NiAs crystal bonds. Mirrors the connectivity baked into GeometryEngine.NiAsCrystal.
    //
    // 8 Fe + 8 S layout (the only variant currently used):
    //   XZ corners  fe0–fe3 (bottom layer), fe4–fe7 (top layer)
    //   XZ edges    s0–s3   (outer S below fe0–fe3), s4–s7 (shared S between layers)
    //   fe0–fe3 each bond to 2 outer S + 2 shared S (4-fold coordination)
    //   fe4–fe7 each bond to 2 shared S only (2-fold, surface layer)
    private static List<BondSpec> NiAsCrystal(List<AtomSpec> atoms)
    {
        var feAtoms = atoms.Where(a => a.Element != "S").ToList();
        var sAtoms  = atoms.Where(a => a.Element == "S").ToList();

        if (feAtoms.Count == 8 && sAtoms.Count == 8)
        {
            return [
                // fe0–fe3 → outer S (s0–s3) below
                new(feAtoms[0].Id, sAtoms[0].Id, "ionic"), new(feAtoms[0].Id, sAtoms[3].Id, "ionic"),
                new(feAtoms[1].Id, sAtoms[0].Id, "ionic"), new(feAtoms[1].Id, sAtoms[1].Id, "ionic"),
                new(feAtoms[2].Id, sAtoms[2].Id, "ionic"), new(feAtoms[2].Id, sAtoms[3].Id, "ionic"),
                new(feAtoms[3].Id, sAtoms[1].Id, "ionic"), new(feAtoms[3].Id, sAtoms[2].Id, "ionic"),
                // fe0–fe3 → shared S (s4–s7) above
                new(feAtoms[0].Id, sAtoms[4].Id, "ionic"), new(feAtoms[0].Id, sAtoms[7].Id, "ionic"),
                new(feAtoms[1].Id, sAtoms[4].Id, "ionic"), new(feAtoms[1].Id, sAtoms[5].Id, "ionic"),
                new(feAtoms[2].Id, sAtoms[6].Id, "ionic"), new(feAtoms[2].Id, sAtoms[7].Id, "ionic"),
                new(feAtoms[3].Id, sAtoms[5].Id, "ionic"), new(feAtoms[3].Id, sAtoms[6].Id, "ionic"),
                // fe4–fe7 → shared S (s4–s7)
                new(feAtoms[4].Id, sAtoms[4].Id, "ionic"), new(feAtoms[4].Id, sAtoms[7].Id, "ionic"),
                new(feAtoms[5].Id, sAtoms[4].Id, "ionic"), new(feAtoms[5].Id, sAtoms[5].Id, "ionic"),
                new(feAtoms[6].Id, sAtoms[6].Id, "ionic"), new(feAtoms[6].Id, sAtoms[7].Id, "ionic"),
                new(feAtoms[7].Id, sAtoms[5].Id, "ionic"), new(feAtoms[7].Id, sAtoms[6].Id, "ionic"),
            ];
        }

        return [];
    }
}
