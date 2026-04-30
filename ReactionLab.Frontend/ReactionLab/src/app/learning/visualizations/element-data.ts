export interface ElementData {
  color: string;
  radius: number;
}

export const ELEMENT_DATA: Record<string, ElementData> = {
  'H':  { color: '#9AB5EA', radius: 0.37 }, // Covalent radius ~0.37 Å
  'O':  { color: '#FF4444', radius: 0.66 }, // Covalent radius ~0.66 Å
  'Fe': { color: '#B3B3B3', radius: 1.24 }, // Metallic/Covalent radius ~1.24 Å
  'S':  { color: '#F0D807', radius: 1.05 }, // Covalent radius ~1.05 Å
  'C':  { color: '#222222', radius: 0.76 }, // Covalent radius ~0.76 Å
  'N':  { color: '#3366FF', radius: 0.71 }, // Covalent radius ~0.71 Å
  'Na': { color: '#AB82FF', radius: 1.60 }, // Empirical radius ~1.60 Å
  'Cl': { color: '#A0F2A1', radius: 1.02 }, // Covalent radius ~1.02 Å
};

export const DEFAULT_ELEMENT_DATA: ElementData = { color: '#888888', radius: 0.5 };

// ── Space-fill (Van der Waals) radii ─────────────────────────────────────────
// Experimental values in Ångströms. Increase VDW_SCALE to make all space-fill
// atoms bigger uniformly; adjust individual values to change relative sizes.
// 1 Three.js unit ≈ 1.67 Å  (Scale = 0.6 in GeometryEngine)
export const VDW_SCALE = 0.5;

export const ELEMENT_VDW: Record<string, number> = {
  'H':  1.20 * VDW_SCALE,
  'O':  1.52 * VDW_SCALE,
  'Fe': 2.05 * VDW_SCALE,
  'S':  1.80 * VDW_SCALE,
  'C':  1.70 * VDW_SCALE,
  'N':  1.55 * VDW_SCALE,
  'Na': 2.27 * VDW_SCALE,
  'Cl': 1.75 * VDW_SCALE,
};

export const DEFAULT_VDW = 1.50 * VDW_SCALE;

// ── Ball-and-stick ────────────────────────────────────────────────────────────
// All atoms rendered at the same radius regardless of element.
export const BALL_STICK_RADIUS = 0.4;
