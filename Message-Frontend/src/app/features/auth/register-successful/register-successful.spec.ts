import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterSuccessful } from './register-successful';

describe('RegisterSuccesful', () => {
  let component: RegisterSuccessful;
  let fixture: ComponentFixture<RegisterSuccessful>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterSuccessful],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterSuccessful);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
