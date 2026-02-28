import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeaturessectionComponent } from './featuressection.component';

describe('FeaturessectionComponent', () => {
  let component: FeaturessectionComponent;
  let fixture: ComponentFixture<FeaturessectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeaturessectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeaturessectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
