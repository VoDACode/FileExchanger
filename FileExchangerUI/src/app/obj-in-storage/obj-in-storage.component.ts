import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ContextMenuModel } from 'src/models/ContextMenuModel';

@Component({
  selector: 'app-obj-in-storage',
  templateUrl: './obj-in-storage.component.html',
  styleUrls: ['./obj-in-storage.component.css']
})
export class ObjInStorageComponent {
  @Input()
  name: string = "";
  @Input()
  key: string = "";
  @Input()
  type: 'FILE' | 'FOLDER' = "FILE";
  @Input()
  isRenamed: boolean = false;
  @Output()
  onopen: EventEmitter<ObjInStorageComponent> = new EventEmitter();
  @Output()
  onopencontextmenu: EventEmitter<{event: any, item: ObjInStorageComponent}> = new EventEmitter();
  @Output()
  onrename: EventEmitter<ObjInStorageComponent> = new EventEmitter();
  public get path(): string { 
    if(this.type === 'FILE')
      return "./assets/icons8-file-100.svg";
    if(this.type === 'FOLDER')
      return "./assets/icons8-folder-100.svg";
    return '';
  }
  get shortName(): string{
    if(this.name.length >= 16)
      return this.name.substring(0, 13) + '...';
    return this.name;
  }

  constructor(private router: Router) {
  }

  openContextMenu(event: any): boolean{
    this.onopencontextmenu.emit({event: event, item: this});
    return false;
  }

  unselect(event: any = undefined): void {
    if(event.key === 'Enter' || !event){
      if(this.name.length === 0){
        alert("Incorrect name!");
        return;
      }
      this.isRenamed = false;
      this.onrename.emit(this);
    }
  }

  open(): void {
    if(this.isRenamed)
      return;
    if(this.type === 'FOLDER' && this.key != '#new_folder' && !this.isRenamed){
      this.router.navigate(['/my/s', this.key]);
    }
    this.onopen.emit(this);
  }
}
