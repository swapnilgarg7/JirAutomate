import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Transcripts } from './transcripts';

describe('Transcripts', () => {
  let component: Transcripts;
  let fixture: ComponentFixture<Transcripts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Transcripts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Transcripts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
