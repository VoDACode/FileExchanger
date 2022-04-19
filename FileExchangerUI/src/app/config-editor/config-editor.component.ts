import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigModule } from 'src/models/configModel';
import { EventModificationConfigParameter } from 'src/models/eventModificationConfigParameter';
import { AuthService } from 'src/services/auth';
import { ConfigService } from 'src/services/config';
import * as $ from 'jquery';

@Component({
  selector: 'app-config-editor',
  templateUrl: './config-editor.component.html',
  styleUrls: ['./config-editor.component.css', '../../assets/css/content.css']
})
export class ConfigEditorComponent implements OnInit {

  public childrens: ConfigModule[] = [];
  private modificationList: EventModificationConfigParameter[] = [];
  public get isModified(): boolean {return this.modificationList.length > 0;}

  constructor(private router: Router) { }

  ngOnInit(): void {
    if (!AuthService.isAdmin) {
      this.router.navigate(['/']);
      return;
    }
    let data = JSON.parse(ConfigService.getConfig);
    this.childrens = this.fillChildren(data, undefined);
  }

  fillChildren(data: any, root: ConfigModule | undefined): ConfigModule[] {
    let configs: ConfigModule[] = [];
    for (const key in data) {
      if (Object.prototype.hasOwnProperty.call(data, key)) {
        const element = data[key];
        let item = new ConfigModule();
        item.root = root;
        item.parameter = key;
        if(Object.prototype.isPrototypeOf(element)) {
          item.childrens = this.fillChildren(element, item);
        }else{
          item.value = data[key]; 
        }
        configs.push(item);
      }
    }
    return configs;
  }

  changetEvent(event: EventModificationConfigParameter): void {
    let index = this.modificationList.findIndex(p => p.path == event.path);
    if(index < 0){
      this.modificationList.push(event);
    }else{
      if(this.modificationList[index].value === event.value){
        return;
      }else if(this.modificationList[index].lastValue === event.value){
        this.modificationList = this.modificationList.slice(index, 0);
      }else{
        this.modificationList[index].value = event.value;
      }
    }
  }

  saveChanges() {
    for(let i = 0; i < this.modificationList.length; i++){
      let item = this.modificationList[i];
      $.ajax({
        method: "POST",
        url: `api/config/set?p=${item.path}&v=${item.value}`,
        async: false,
        headers:{
          Authorization: "Bearer " + AuthService.token
        }
      });
      item.target.value = item.value;
    }
    $.ajax({
      method: "POST",
      url: `api/config/reload`,
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      success: data => {
        this.modificationList.length = 0;
      }
    });
  }

  canselChanged(): void {
    for(let i = 0; i < this.modificationList.length; i++){
      this.modificationList[i].target.value = this.modificationList[i].lastValue;
    }
    this.modificationList.length = 0;
  }

}
