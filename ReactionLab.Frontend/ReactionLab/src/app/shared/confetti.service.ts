import { Injectable } from '@angular/core';

interface Particle {
  x: number; y: number;
  vx: number; vy: number;
  color: string;
  size: number;
  rotation: number;
  rotSpeed: number;
  life: number;
}

@Injectable({ providedIn: 'root' })
export class ConfettiService {
  private readonly colors = ['#5c79a7', '#A75C87', '#5cb87a', '#d4922a', '#e85d5d', '#FFF8F0'];

  burst(originX = window.innerWidth / 2, originY = window.innerHeight / 3): void {
    const canvas = document.createElement('canvas');
    canvas.style.cssText = 'position:fixed;top:0;left:0;width:100%;height:100%;pointer-events:none;z-index:99999';
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    document.body.appendChild(canvas);

    const ctx = canvas.getContext('2d')!;
    const particles: Particle[] = Array.from({ length: 80 }, () => {
      const angle = Math.random() * Math.PI * 2;
      const speed = 4 + Math.random() * 8;
      return {
        x: originX, y: originY,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed - 4,
        color: this.colors[Math.floor(Math.random() * this.colors.length)],
        size: 5 + Math.random() * 6,
        rotation: Math.random() * 360,
        rotSpeed: (Math.random() - 0.5) * 8,
        life: 1
      };
    });

    let frame: number;
    const tick = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      let alive = false;
      for (const p of particles) {
        p.x += p.vx;
        p.y += p.vy;
        p.vy += 0.25;
        p.vx *= 0.98;
        p.rotation += p.rotSpeed;
        p.life -= 0.018;
        if (p.life <= 0) continue;
        alive = true;
        ctx.save();
        ctx.globalAlpha = p.life;
        ctx.translate(p.x, p.y);
        ctx.rotate((p.rotation * Math.PI) / 180);
        ctx.fillStyle = p.color;
        ctx.fillRect(-p.size / 2, -p.size / 4, p.size, p.size / 2);
        ctx.restore();
      }
      if (alive) {
        frame = requestAnimationFrame(tick);
      } else {
        canvas.remove();
      }
    };
    frame = requestAnimationFrame(tick);
    setTimeout(() => { cancelAnimationFrame(frame); canvas.remove(); }, 4000);
  }
}
