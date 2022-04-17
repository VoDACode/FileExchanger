import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Confirm.EmailComponent } from './confirm.email.component';

describe('Confirm.EmailComponent', () => {
  let component: Confirm.EmailComponent;
  let fixture: ComponentFixture<Confirm.EmailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Confirm.EmailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Confirm.EmailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
