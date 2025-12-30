import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupOptions } from './group-options';

describe('GroupOptions', () => {
  let component: GroupOptions;
  let fixture: ComponentFixture<GroupOptions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupOptions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupOptions);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
