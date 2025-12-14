import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileLayout } from './profile-layout';

describe('ProfileLayout', () => {
  let component: ProfileLayout;
  let fixture: ComponentFixture<ProfileLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfileLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
