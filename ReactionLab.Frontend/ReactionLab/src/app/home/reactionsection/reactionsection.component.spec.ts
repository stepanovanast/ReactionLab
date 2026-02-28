import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReactionsectionComponent } from './reactionsection.component';

describe('ReactionsectionComponent', () => {
  let component: ReactionsectionComponent;
  let fixture: ComponentFixture<ReactionsectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactionsectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReactionsectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
