import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/services/auth';
import * as $ from 'jquery';
import { FileTreeModel } from 'src/models/fileInTreeModel';
import { DirectoryService } from 'src/services/directory';
import { ContextMenuModel } from 'src/models/ContextMenuModel';
import { ObjInStorageComponent } from '../obj-in-storage/obj-in-storage.component';

@Component({
  selector: 'app-storage-page',
  templateUrl: './storage-page.component.html',
  styleUrls: ['./storage-page.component.css']
})
export class StoragePageComponent implements OnInit {
  contextMenu: { pos: { x: number, y: number }, isView: boolean, items: ContextMenuModel[] } = { pos: { x: 0, y: 0 }, isView: false, items: [] }
  contextMenuItems: ContextMenuModel[][] = [];
  folders: [] = [];
  dirContent: ObjInStorageModel[] = [];
  selectItem: ObjInStorageComponent | undefined;
  rootKey: string = DirectoryService.getRootKey;
  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    this.activatedRoute.params.subscribe(params => {
      if (params.dir === undefined) {
        this.rootKey = DirectoryService.getRootKey;
      } else{
        this.rootKey = params.dir;
      }
      this.updateContect(this.rootKey);
    });
    $.ajax({
      method: 'GET',
      url: 'api/ui/auth/storage/use',
      success: (data: any) => {
        if (data && !AuthService.isAuth()) {
          this.router.navigate(['/auth']);
        }
      }
    });
  }

  ngOnInit(): void {
    this.contextMenuItems.push([
      new ContextMenuModel("Create new dir", this.onCreateFolder.bind(this), true),
      new ContextMenuModel("Download", this.onDownloadThisFolder.bind(this), true),
      new ContextMenuModel("Propertis", this.onClickContextMenu.bind(this), true),
    ]);
    this.contextMenuItems.push([
      new ContextMenuModel("Rename", this.onRename.bind(this), true),
      new ContextMenuModel("Download", this.onDownloadSelectItem.bind(this), true),
      new ContextMenuModel("Delete", this.onDeleteObj.bind(this), true),
      new ContextMenuModel("Propertis", this.onClickContextMenu.bind(this), true)
    ]);
  }

  onDownloadSelectItem(event: Event, item: ContextMenuModel): void {
    if (this.selectItem?.type === 'FOLDER'){
      //@ts-ignore
      this.onDownloadFolder(this.selectItem?.key);
      return;
    }else if (this.selectItem?.type === 'FILE'){
      this.onDownloadFile();
      return;
    }
  }
  onDownloadThisFolder(event: Event, item: ContextMenuModel): void {
    this.onDownloadFolder(this.rootKey);
  }

  private onDownloadFolder(key: string): void {
    $.ajax({
      method: "GET",
      url: `api/dir/s/${key}/download`,   
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      success: (data) => {
        document.location.replace(data);
      }
    });
  }
  
  private onDownloadFile(): void {
    $.ajax({
      method: "POST",
      url: `api/files/s/${this.rootKey}/${this.selectItem?.key}/download`,
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      // TO DO
      processData: false,
    });
  }

  dragOverHandler(ev: any): void {
    console.log('File(s) in drop zone');
  
    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();
  }
  
  dropHandler(ev: any): void {
    console.log('File(s) dropped');

    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();
  
    if (ev.dataTransfer.items) {
      // Use DataTransferItemList interface to access the file(s)
      for (var i = 0; i < ev.dataTransfer.items.length; i++) {
        // If dropped items aren't files, reject them
        if (ev.dataTransfer.items[i].kind === 'file') {
          var file = ev.dataTransfer.items[i].getAsFile();
          console.log('... file[' + i + '].name = ' + file.name);
          this.upload(file);
        }
      }
    } else {
      // Use DataTransfer interface to access the file(s)
      for (var i = 0; i < ev.dataTransfer.files.length; i++) {
        console.log('... file[' + i + '].name = ' + ev.dataTransfer.files[i].name);
        this.upload(ev.dataTransfer.files[i]);
      }
    }
  }

  private upload(file: File): void {
    let url = `/api/files/s/${this.rootKey}/upload`;
    const form = new FormData();
    console.log(file);
    console.log(form);
    form.append("file", file, file.name);
    $.ajax({
      url: url,
      type: 'POST',
      data: form,
      headers: {
        Authorization: 'Bearer ' + AuthService.token
      },
      success: (response) => {
        console.log(response);
        this.updateContect(this.rootKey);
      },
      error: (response) => {
        console.log(response);
      },
      processData: false,
      contentType: false,
    });
  }

  onDeleteObj(event: Event, item: ContextMenuModel): void {
    let url = this.selectItem?.type == 'FILE' ? `api/files/s/${this.rootKey}/${this.selectItem?.key}/delete` : 
    `api/dir/s/${this.selectItem?.key}/delete`;
    $.ajax({
      method: 'DELETE',
      url: url,
      headers: {
        Authorization: "Bearer " + AuthService.token
      },
      success: (data) =>{
        this.updateContect(this.rootKey);
      }
    })
  }

  onCreateFolder(event: Event, item: ContextMenuModel): void {
    this.dirContent.push(new ObjInStorageModel("#new_folder", "New Folder", true, 'FOLDER', this.rootKey));
  }

  renameEvent(event: ObjInStorageComponent): void {
    if (event.key == '#new_folder') {
      $.ajax({
        method: "POST",
        url: `api/dir/s/${this.rootKey}/create?name=${event.name}`,
        headers: {
          Authorization: "Bearer " + AuthService.token
        },
        success: (data) => {
          this.updateContect(this.rootKey);
        }
      })
    }else{
      let url = (event.type == 'FOLDER' ? `/api/dir/s/${event.key}` : `/api/files/s/${this.rootKey}/${event.key}`) + `/rename?name=${event.name}`;
      $.ajax({
        method: "POST",
        url: url,
        headers: {
          Authorization: "Bearer " + AuthService.token
        },
        success: (data) => {
          this.updateContect(this.rootKey);
        }
      })
    }
  }

  updateContect(dir: string): void {
    let list = DirectoryService.list(dir);
    this.dirContent = [];
    for (let i = 0; i < list.length; i++) {
      this.dirContent.push(new ObjInStorageModel(list[i].key, list[i].name, false, list[i].isFile ? 'FILE' : 'FOLDER', this.rootKey));
    }
  }

  onRename(event: Event, item: ContextMenuModel): void {
    if (!this.selectItem)
      return;
    this.selectItem.isRenamed = true;
  }

  onClickContextMenu(event: Event, item: ContextMenuModel): void {
    
  }

  openContextMenu(event: any): boolean {
    this.contextMenu.items = this.contextMenuItems[0];
    this.contextMenu.pos.x = event.clientX;
    this.contextMenu.pos.y = event.clientY;
    this.contextMenu.isView = true;
    return false;
  }

  onOpenContextMenuInFile(data: { event: any; item: ObjInStorageComponent; }): boolean {
    this.contextMenu.items = this.contextMenuItems[1];
    this.selectItem = data.item;
    this.contextMenu.pos.x = data.event.clientX;
    this.contextMenu.pos.y = data.event.clientY;
    this.contextMenu.isView = true;
    return false;
  }

  onCloseContextMenu(): void {
    this.selectItem = undefined;
    this.contextMenu.isView = false;
  }
}

export class ObjInStorageModel {
  key: string = "";
  name: string = "";
  type: 'FILE' | 'FOLDER' = "FILE";
  isRenamed: boolean = false;
  rootKey: string = "";
  constructor(key: string, name: string, isRenamed: boolean, type: 'FILE' | 'FOLDER', rootKey: string) {
    this.isRenamed = isRenamed;
    this.key = key;
    this.name = name;
    this.type = type;
    this.rootKey = rootKey;
  }
}