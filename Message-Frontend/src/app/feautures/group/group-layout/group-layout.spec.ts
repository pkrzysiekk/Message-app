import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupLayout } from './group-layout';

describe('GroupLayout', () => {
  let component: GroupLayout;
  let fixture: ComponentFixture<GroupLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
