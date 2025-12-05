import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeLayout } from './home-layout';

describe('HomeLayout', () => {
  let component: HomeLayout;
  let fixture: ComponentFixture<HomeLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomeLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
