import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ContextMenuModel } from 'src/models/ContextMenuModel';
import * as $ from 'jquery';

@Component({
  selector: 'app-storage-context-menu',
  templateUrl: './storage-context-menu.component.html',
  styleUrls: ['./storage-context-menu.component.css']
})
export class StorageContextMenuComponent{
  @Input()
  items: ContextMenuModel[] = [];
  @Input()
  x: number = 0;
  @Input() 
  y: number = 0;
  @Output()
  close: EventEmitter<void> = new EventEmitter()
  constructor() { 
  }
  selectItem(item: ContextMenuModel, event: Event): void {
    item.click(event, item);
    if(item.thenClosed){
      this.close.emit();
    }
  }
  closeEvent(): boolean {
    this.close.emit();
    return false;
  }
}