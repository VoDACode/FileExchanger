import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmTelegramComponent } from './confirm-telegram.component';

describe('ConfirmTelegramComponent', () => {
  let component: ConfirmTelegramComponent;
  let fixture: ComponentFixture<ConfirmTelegramComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfirmTelegramComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmTelegramComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
