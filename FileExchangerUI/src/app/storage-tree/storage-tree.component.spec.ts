import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StorageTreeComponent } from './storage-tree.component';

describe('StorageTreeComponent', () => {
  let component: StorageTreeComponent;
  let fixture: ComponentFixture<StorageTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StorageTreeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StorageTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
