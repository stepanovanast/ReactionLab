import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WhysectionComponent } from './whysection.component';

describe('WhysectionComponent', () => {
  let component: WhysectionComponent;
  let fixture: ComponentFixture<WhysectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WhysectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WhysectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
