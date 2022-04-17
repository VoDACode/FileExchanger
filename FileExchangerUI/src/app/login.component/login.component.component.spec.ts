import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Login.ComponentComponent } from './login.component.component';

describe('Login.ComponentComponent', () => {
  let component: Login.ComponentComponent;
  let fixture: ComponentFixture<Login.ComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Login.ComponentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Login.ComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
