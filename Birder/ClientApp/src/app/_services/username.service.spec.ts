import { TestBed } from '@angular/core/testing';

import { UsernameService } from './username.service';

describe('UsernameService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UsernameService = TestBed.inject(UsernameService);
    expect(service).toBeTruthy();
  });
});
