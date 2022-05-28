import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { ObjInStorageModel } from '../storage-page/storage-page.component';
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';
import { SizeHelper } from 'src/helpers/SizeHelper';

@Component({
  selector: 'app-storage-object-properties-component',
  templateUrl: './storage-object-properties-component.component.html',
  styleUrls: ['./storage-object-properties-component.component.css']
})
export class StorageObjectPropertiesComponentComponent implements AfterViewInit{
  @Input()
  item: ObjInStorageModel | undefined;
  size = 0;
  createDate = "";
  updateDate = "";
  colSize = 5;
  public get sizeString(): string {
    return SizeHelper.toString(this.size);
  }
  constructor() { }

  ngAfterViewInit(): void {
    $.ajax({
      method: "GET",
      url: `api/files/s/${this.item?.rootKey}/${this.item?.key}/info`,
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      success: (data) => {
        this.size = data.size;
        this.createDate = data.createDate;
        this.updateDate = data.updateDate;
      }
    });
  }

}
