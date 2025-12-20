import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Groups } from './groups';

describe('Group', () => {
  let component: Groups;
  let fixture: ComponentFixture<Groups>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Groups],
    }).compileComponents();

    fixture = TestBed.createComponent(Groups);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
