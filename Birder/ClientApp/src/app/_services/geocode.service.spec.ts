import { TestBed } from '@angular/core/testing';

import { GeocodeService } from './geocode.service';

describe('GeocodeService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GeocodeService = TestBed.inject(GeocodeService);
    expect(service).toBeTruthy();
  });
});
