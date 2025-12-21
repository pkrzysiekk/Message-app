import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupView } from './group-view';

describe('Group', () => {
  let component: GroupView;
  let fixture: ComponentFixture<GroupView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupView],
    }).compileComponents();

    fixture = TestBed.createComponent(GroupView);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
