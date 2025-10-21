import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrimengTestComponent } from './primeng-test.component';

describe('PrimengTestComponent', () => {
  let component: PrimengTestComponent;
  let fixture: ComponentFixture<PrimengTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrimengTestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrimengTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
