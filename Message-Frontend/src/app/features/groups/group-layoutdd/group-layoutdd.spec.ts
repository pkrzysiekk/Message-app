import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupLayoutdd } from './group-layoutdd';

describe('GroupLayoutdd', () => {
  let component: GroupLayoutdd;
  let fixture: ComponentFixture<GroupLayoutdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupLayoutdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupLayoutdd);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
