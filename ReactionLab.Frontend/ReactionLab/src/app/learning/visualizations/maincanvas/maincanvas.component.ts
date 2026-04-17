import { Component, ElementRef, EventEmitter, HostListener, Input, OnChanges, AfterViewInit, OnDestroy, Output, SimpleChanges, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { ApiService, Reaction, ReactionStep, Atom, Bond } from '../../../services/api.service';

const ELEMENT_PROPS: Record<string, { color: number; radius: number }> = {
  'H':  { color: 0xEEEEEE, radius: 0.35 },
  'O':  { color: 0xFF4444, radius: 0.55 },
  'Fe': { color: 0xB8B8B8, radius: 0.75 },
  'S':  { color: 0xFFCC00, radius: 0.60 },
  'C':  { color: 0x222222, radius: 0.45 },
  'N':  { color: 0x3366FF, radius: 0.50 },
};
const DEFAULT_PROPS = { color: 0x888888, radius: 0.5 };

// Van der Waals radii in Ångströms (experimental values).
// Used by space-filling mode — atoms are sized accurately relative to each other.
// Scale factor 0.6 matches GeometryEngine (1 Three.js unit ≈ 1.67 Å).
const ELEMENT_VDW: Record<string, number> = {
  'H':  1.20 * 0.6,
  'O':  1.52 * 0.6,
  'Fe': 2.05 * 0.6,
  'S':  1.80 * 0.6,
  'C':  1.70 * 0.6,
  'N':  1.55 * 0.6,
};
const DEFAULT_VDW = 1.50 * 0.6;

// In ball-and-stick all atoms are rendered at the same small radius
const BALL_STICK_RADIUS = 0.4;

type ViewMode = 'ball-stick' | 'space-fill';

@Component({
  selector: 'app-maincanvas',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './maincanvas.component.html',
  styleUrl: './maincanvas.component.css'
})
export class MaincanvasComponent implements AfterViewInit, OnChanges, OnDestroy {
  @ViewChild('threeCanvas', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;

  @Input() reaction: Reaction | null = null;
  @Input() currentStepIndex = 0;
  @Input() step: ReactionStep | null = null;
  @Output() stepChange = new EventEmitter<number>();

  displayedTemperature = 0;
  playing = false;
  viewMode: ViewMode = 'ball-stick';

  private apiService = inject(ApiService);
  private hasAwardedMolecularVision = false;
  private hasAwardedFullCircuit = false;

  private activeStep: ReactionStep | null = null;

  get totalSteps(): number { return this.reaction?.steps?.length ?? 0; }
  get progressPercent(): number {
    return this.totalSteps > 1 ? (this.currentStepIndex / (this.totalSteps - 1)) * 100 : 0;
  }

  private playTimer?: ReturnType<typeof setInterval>;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private controls!: OrbitControls;
  private atomMeshes = new Map<string, THREE.Mesh>();
  private bondMeshes: THREE.Mesh[] = [];
  private renderLoopId = 0;
  private tweenId = 0;
  private tempTweenId = 0;
  private glowAnimId = 0;
  private initialized = false;
  private resizeObserver?: ResizeObserver;

  ngAfterViewInit(): void {
    // Defer until after browser layout pass so clientWidth/clientHeight are non-zero
    setTimeout(() => {
      this.initThree();
      this.renderLoop();
      this.initialized = true;

      if (this.step) {
        this.buildAtoms(this.step.atoms);
        this.buildBonds(this.step.bonds, this.step.atoms);
        this.applyAtomOverrides(this.step);
        this.displayedTemperature = this.step.temperatureRange?.start ?? (this.reaction?.temperature ?? 0);
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.initialized) return;

    if (changes['step']) {
      const prev: ReactionStep | null = changes['step'].previousValue;
      const curr: ReactionStep | null = changes['step'].currentValue;
      if (curr) this.transitionToStep(prev, curr);
    }

    if (changes['currentStepIndex']) {
      // Full Circuit badge is awarded by the parent VisualizationsComponent
    }
  }

  @HostListener('window:keydown.space', ['$event'])
  onSpacebar(event: KeyboardEvent): void {
    event.preventDefault(); // prevent page scroll
    this.togglePlay();
  }

  togglePlay(): void {
    if (this.playing) {
      this.stopPlay();
    } else {
      this.startPlay();
    }
  }

  setViewMode(mode: ViewMode): void {
    if (this.viewMode === mode) return;
    this.viewMode = mode;
    // Refresh all atom sizes and bond visibility for the new mode
    this.resetAtomVisuals();
    if (this.activeStep) this.applyAtomOverrides(this.activeStep);
    for (const bond of this.bondMeshes) {
      bond.visible = mode === 'ball-stick';
    }
    // Award Molecular Vision badge on first mode switch
    if (!this.hasAwardedMolecularVision) {
      this.hasAwardedMolecularVision = true;
      this.apiService.awardMolecularVision().subscribe();
    }
  }

  private startPlay(): void {
    this.playing = true;
    this.playTimer = setInterval(() => {
      const next = this.currentStepIndex + 1;
      if (next >= this.totalSteps) {
        this.stopPlay();
      } else {
        this.stepChange.emit(next);
      }
    }, 4000);
  }

  private stopPlay(): void {
    this.playing = false;
    if (this.playTimer !== undefined) {
      clearInterval(this.playTimer);
      this.playTimer = undefined;
    }
  }

  ngOnDestroy(): void {
    this.stopPlay();
    cancelAnimationFrame(this.renderLoopId);
    cancelAnimationFrame(this.tweenId);
    cancelAnimationFrame(this.tempTweenId);
    cancelAnimationFrame(this.glowAnimId);
    this.resizeObserver?.disconnect();
    window.removeEventListener('resize', this.onResize);
    this.renderer?.dispose();
  }

  private initThree(): void {
    const canvas = this.canvasRef.nativeElement;
    // Read from parent container — canvas itself may not have layout dimensions yet
    const container = canvas.parentElement!;
    const w = container.clientWidth || 600;
    const h = container.clientHeight || 600;

    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0xFFF8F0);

    this.camera = new THREE.PerspectiveCamera(50, w / h, 0.1, 100);
    this.camera.position.set(0, 2, 12);

    this.renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
    this.renderer.setSize(w, h);
    this.renderer.setPixelRatio(window.devicePixelRatio);

    this.scene.add(new THREE.AmbientLight(0xffffff, 0.6));
    const sun = new THREE.DirectionalLight(0xffffff, 0.9);
    sun.position.set(5, 10, 7);
    this.scene.add(sun);

    this.controls = new OrbitControls(this.camera, canvas);
    this.controls.enableDamping = true;
    this.controls.dampingFactor = 0.05;
    this.controls.minDistance = 3;
    this.controls.maxDistance = 20;

    window.addEventListener('resize', this.onResize);

    // Also watch container for size changes (e.g. sidebar collapse)
    this.resizeObserver = new ResizeObserver(() => this.onResize());
    this.resizeObserver.observe(container);
  }

  private renderLoop = (): void => {
    this.renderLoopId = requestAnimationFrame(this.renderLoop);
    this.controls?.update();
    this.renderer?.render(this.scene, this.camera);
  };

  private transitionToStep(prev: ReactionStep | null, curr: ReactionStep): void {
    cancelAnimationFrame(this.glowAnimId);

    this.activeStep = curr;

    if (!prev) {
      this.buildAtoms(curr.atoms);
      this.buildBonds(curr.bonds, curr.atoms);
      this.applyAtomOverrides(curr);
      this.animateTemperature(curr.temperatureRange?.start ?? 0, curr.temperatureRange?.end ?? 0);
      return;
    }

    this.clearBonds();
    this.animateTemperature(curr.temperatureRange?.start ?? 0, curr.temperatureRange?.end ?? 0);

    // Add new atoms that didn't exist before
    for (const atom of curr.atoms) {
      if (!this.atomMeshes.has(atom.id)) {
        this.addAtomMesh(atom);
      }
    }

    // Remove atoms that left the scene
    const currIds = new Set(curr.atoms.map((a: Atom) => a.id));
    for (const [id, mesh] of this.atomMeshes) {
      if (!currIds.has(id)) {
        this.scene.remove(mesh);
        (mesh.geometry as THREE.BufferGeometry).dispose();
        this.atomMeshes.delete(id);
      }
    }

    // Reset visuals before animating
    this.resetAtomVisuals();

    const startPos = new Map<string, THREE.Vector3>(
      Array.from(this.atomMeshes.entries()).map(([id, m]) => [id, m.position.clone()])
    );
    const targetPos = new Map<string, THREE.Vector3>(
      curr.atoms.map((a: Atom) => [a.id, new THREE.Vector3(a.x, a.y, a.z)])
    );

    const duration = 900;
    const start = performance.now();

    const tween = (now: number) => {
      const t = Math.min((now - start) / duration, 1);
      const eased = t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t;

      for (const [id, mesh] of this.atomMeshes) {
        const from = startPos.get(id);
        const to = targetPos.get(id);
        if (from && to) mesh.position.lerpVectors(from, to, eased);
      }

      if (t < 1) {
        this.tweenId = requestAnimationFrame(tween);
      } else {
        this.buildBonds(curr.bonds, curr.atoms);
        this.applyAtomOverrides(curr);
        this.animateGlow(curr);
      }
    };

    cancelAnimationFrame(this.tweenId);
    this.tweenId = requestAnimationFrame(tween);
  }

  private buildAtoms(atoms: Atom[]): void {
    for (const atom of atoms) {
      if (!this.atomMeshes.has(atom.id)) {
        this.addAtomMesh(atom);
      } else {
        this.atomMeshes.get(atom.id)!.position.set(atom.x, atom.y, atom.z);
      }
    }
  }

  private addAtomMesh(atom: Atom): void {
    const props = ELEMENT_PROPS[atom.element] ?? DEFAULT_PROPS;
    // Always build geometry at the covalent radius; scale handles the rest
    const geo = new THREE.SphereGeometry(props.radius, 32, 32);
    const mat = new THREE.MeshPhongMaterial({ color: props.color, shininess: 90 });
    const mesh = new THREE.Mesh(geo, mat);
    mesh.position.set(atom.x, atom.y, atom.z);
    mesh.scale.setScalar(this.getBaseScale(atom.element));
    mesh.userData['element'] = atom.element;
    this.scene.add(mesh);
    this.atomMeshes.set(atom.id, mesh);
  }

  // Base scale for the current view mode, before any per-atom overrides.
  // Ball-stick: uniform small spheres (scale drives radius down to BALL_STICK_RADIUS).
  // Space-fill: each element scaled to its van der Waals radius.
  private getBaseScale(element: string): number {
    const props = ELEMENT_PROPS[element] ?? DEFAULT_PROPS;
    if (this.viewMode === 'space-fill') {
      return (ELEMENT_VDW[element] ?? DEFAULT_VDW) / props.radius;
    }
    return BALL_STICK_RADIUS / props.radius;
  }

  private clearBonds(): void {
    for (const b of this.bondMeshes) {
      this.scene.remove(b);
      (b.geometry as THREE.BufferGeometry).dispose();
    }
    this.bondMeshes = [];
  }

  private buildBonds(bonds: Bond[], atoms: Atom[]): void {
    this.clearBonds();
    if (this.viewMode === 'space-fill') return; // no bonds in space-fill mode
    const atomMap = new Map(atoms.map(a => [a.id, a]));

    for (const bond of bonds) {
      const a1 = atomMap.get(bond.from);
      const a2 = atomMap.get(bond.to);
      if (!a1 || !a2) continue;

      const v1 = new THREE.Vector3(a1.x, a1.y, a1.z);
      const v2 = new THREE.Vector3(a2.x, a2.y, a2.z);
      const dir = new THREE.Vector3().subVectors(v2, v1);
      const length = dir.length();
      const mid = new THREE.Vector3().addVectors(v1, v2).multiplyScalar(0.5);

      const geo = new THREE.CylinderGeometry(0.1, 0.1, length, 16);
      const mat = new THREE.MeshPhongMaterial({ color: 0x666666 });
      const cylinder = new THREE.Mesh(geo, mat);
      cylinder.position.copy(mid);
      cylinder.quaternion.setFromUnitVectors(new THREE.Vector3(0, 1, 0), dir.normalize());
      this.scene.add(cylinder);
      this.bondMeshes.push(cylinder);
    }
  }

  private resetAtomVisuals(): void {
    for (const [, mesh] of this.atomMeshes) {
      const element = mesh.userData['element'] as string;
      const props = ELEMENT_PROPS[element] ?? DEFAULT_PROPS;
      const mat = mesh.material as THREE.MeshPhongMaterial;
      mat.color.setHex(props.color);
      mat.emissive.setHex(0x000000);
      mat.emissiveIntensity = 0;
      mesh.scale.setScalar(this.getBaseScale(element));
    }
  }

  private applyAtomOverrides(step: ReactionStep): void {
    const overrides = step.atomOverrides ?? {};
    for (const [id, mesh] of this.atomMeshes) {
      const override = overrides[id];
      if (!override) continue;

      const mat = mesh.material as THREE.MeshPhongMaterial;
      if (override.radiusScale !== undefined) {
        const element = mesh.userData['element'] as string;
        this.animateScale(mesh, this.getBaseScale(element) * override.radiusScale, 600);
      }
      if (override.dimmed) {
        mat.color.setHex(0x777777);
        mat.emissive.setHex(0x000000);
        mat.emissiveIntensity = 0;
      }
      if (override.glowing) {
        mat.emissive.setHex(0xFFAA00);
        mat.emissiveIntensity = 0.3;
      }
      if (override.darkened) {
        mat.color.setHex(0x3D3520);
        mat.emissive.setHex(0x000000);
        mat.emissiveIntensity = 0;
      }
    }
  }

  private animateScale(mesh: THREE.Mesh, targetScale: number, duration: number): void {
    const startScale = mesh.scale.x;
    const start = performance.now();

    const tick = (now: number) => {
      const t = Math.min((now - start) / duration, 1);
      // ease-out cubic
      const eased = 1 - Math.pow(1 - t, 3);
      mesh.scale.setScalar(startScale + (targetScale - startScale) * eased);
      if (t < 1) requestAnimationFrame(tick);
    };
    requestAnimationFrame(tick);
  }

  // Pulsing emissive glow on atoms that receive electrons in this step.
  // Fades in over 600ms then sustains a gentle sin-wave pulse indefinitely
  // until the next step transition cancels it.
  private animateGlow(step: ReactionStep): void {
    const receiverIds = new Set((step.electronTransfers ?? []).map((t: { from: string; to: string }) => t.to));
    if (receiverIds.size === 0) return;

    const receivers: THREE.MeshPhongMaterial[] = [];
    for (const id of receiverIds) {
      const mesh = this.atomMeshes.get(id);
      if (mesh) {
        const mat = mesh.material as THREE.MeshPhongMaterial;
        mat.emissive.setHex(0xFF8800);
        mat.emissiveIntensity = 0;
        receivers.push(mat);
      }
    }
    if (receivers.length === 0) return;

    const FADE_IN = 600;
    const start = performance.now();

    const tick = (now: number) => {
      const elapsed = now - start;
      const t = Math.min(elapsed / FADE_IN, 1);
      const intensity = t < 1
        ? t * 0.6
        : Math.sin((elapsed - FADE_IN) * 0.003) * 0.15 + 0.5;

      for (const mat of receivers) mat.emissiveIntensity = intensity;
      this.glowAnimId = requestAnimationFrame(tick);
    };

    this.glowAnimId = requestAnimationFrame(tick);
  }

  private animateTemperature(from: number, to: number): void {
    cancelAnimationFrame(this.tempTweenId);
    const duration = 2000;
    const start = performance.now();
    const startTemp = this.displayedTemperature;

    const tick = (now: number) => {
      const t = Math.min((now - start) / duration, 1);
      this.displayedTemperature = Math.round(startTemp + (from - startTemp) * Math.min(t * 3, 1));

      // After snapping to range start, count to end
      if (t > 0.33) {
        const t2 = Math.min((t - 0.33) / 0.67, 1);
        this.displayedTemperature = Math.round(from + (to - from) * t2);
      }

      if (t < 1) {
        this.tempTweenId = requestAnimationFrame(tick);
      } else {
        this.displayedTemperature = to;
      }
    };

    this.tempTweenId = requestAnimationFrame(tick);
  }

  private onResize = (): void => {
    const canvas = this.canvasRef.nativeElement;
    const container = canvas.parentElement!;
    const w = container.clientWidth || 600;
    const h = container.clientHeight || 600;
    this.camera.aspect = w / h;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(w, h);
  };
}
