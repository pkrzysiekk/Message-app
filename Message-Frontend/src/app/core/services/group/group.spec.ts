import { TestBed } from '@angular/core/testing';

import { Group } from './group';

describe('Group', () => {
  let service: Group;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Group);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
