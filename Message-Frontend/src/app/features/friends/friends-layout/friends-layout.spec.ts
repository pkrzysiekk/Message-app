import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FriendsLayout } from './friends-layout';

describe('FriendsLayout', () => {
  let component: FriendsLayout;
  let fixture: ComponentFixture<FriendsLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FriendsLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FriendsLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
