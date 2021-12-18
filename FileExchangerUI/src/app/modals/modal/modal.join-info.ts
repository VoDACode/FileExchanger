import { Component } from '@angular/core';
import { MdbModalRef } from 'mdb-angular-ui-kit/modal';
import {document} from "ngx-bootstrap/utils";
import {TranslateService} from "@ngx-translate/core";
import * as $ from 'jquery';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.join-info.html',
  styleUrls: ['./modal.join-info.css']
})
export class ModalJoinInfo {
  public key: string = "";
  public isCreator: boolean = false;
  constructor(public modalRef: MdbModalRef<ModalJoinInfo>, private translateService: TranslateService) { }
  getJoinUrl(): string{
    return `http://${location.host}/join/${this.key}`;
  }

  recreateJoin(): void{
    if(!this.isCreator)
      return;
    $.ajax({
      url: 'api/group/join/url/re-creation',
      method: 'POST',
      success: data => {
        this.key = data;
      },
      error: jqXHR => {

      }
    })
  }

  copyKey(): void{
    const selBox = document.createElement('textarea');
    selBox.style.position = 'fixed';
    selBox.style.left = '0';
    selBox.style.top = '0';
    selBox.style.opacity = '0';
    selBox.value = this.key;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand('copy');
    document.body.removeChild(selBox);
  }
}
