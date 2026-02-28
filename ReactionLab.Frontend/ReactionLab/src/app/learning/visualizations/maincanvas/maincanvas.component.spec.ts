import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaincanvasComponent } from './maincanvas.component';

describe('MaincanvasComponent', () => {
  let component: MaincanvasComponent;
  let fixture: ComponentFixture<MaincanvasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MaincanvasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MaincanvasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
