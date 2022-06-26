import { Component, Input } from '@angular/core';
import { FileTreeModel } from 'src/models/fileInTreeModel';
import { ActivatedRoute, Router } from '@angular/router';
import { DirectoryService } from 'src/services/directory';

@Component({
  selector: 'app-storage-tree',
  templateUrl: './storage-tree.component.html',
  styleUrls: ['./storage-tree.component.css']
})
export class StorageTreeComponent{
  @Input()
  public key: string = "";
  @Input()
  public name: string = "";
  @Input()
  childrens: FileTreeModel[] = [];
  @Input()
  isOpen: boolean = false;
  @Input()
  isHaveFolders: boolean = true;

  public get _childrens(): FileTreeModel[]{
    if(this.isOpen)
      return this.childrens;
    return [];
  }
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    this.activatedRoute.params.subscribe(params => {
      console.log(params.dir);
    });
  }

  viewTree(): void {
    this.isOpen = !this.isOpen;
    this.childrens = [];
    if(this.isOpen){
      let data = DirectoryService.list(this.key, true);
      for(let i = 0; i < data.length; i++){
        let item = new FileTreeModel();
        item.key = data[i].key;
        item.name = data[i].name;
        item.isHaveFolders = data[i].isHaveFolders;
        item.root = data[i].dir;
        this.childrens.push(item);
      }
    }
  }

  onOpen(): void {
    this.router.navigate(['/my/s', this.key]);
  }
}

