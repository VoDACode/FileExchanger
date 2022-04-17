import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StorageContextMenuComponent } from './storage-context-menu.component';

describe('StorageContextMenuComponent', () => {
  let component: StorageContextMenuComponent;
  let fixture: ComponentFixture<StorageContextMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StorageContextMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StorageContextMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
