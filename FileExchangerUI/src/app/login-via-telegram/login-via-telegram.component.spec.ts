import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginViaTelegramComponent } from './login-via-telegram.component';

describe('LoginViaTelegramComponent', () => {
  let component: LoginViaTelegramComponent;
  let fixture: ComponentFixture<LoginViaTelegramComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LoginViaTelegramComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginViaTelegramComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
