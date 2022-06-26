import { Component, EventEmitter, Output } from '@angular/core';
import { MdbModalRef } from 'mdb-angular-ui-kit/modal';
import {document} from "ngx-bootstrap/utils";
import {TranslateService} from "@ngx-translate/core";
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-modal-share-serrings',
  templateUrl: './moadl.share.settings.html',
  styleUrls: ['./moadl.share.settings.css']
})
export class ModalShareSettings {
  public key: string = "";
  public get getKey(): string { return `${location.origin}/share/${this.key}`; }
  public ondelete = (key: string) => {};
  constructor(public modalRef: MdbModalRef<ModalShareSettings>, private translateService: TranslateService) { }

  copyKey(): void{
    const selBox = document.createElement('textarea');
    selBox.style.position = 'fixed';
    selBox.style.left = '0';
    selBox.style.top = '0';
    selBox.style.opacity = '0';
    selBox.value = this.getKey;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand('copy');
    document.body.removeChild(selBox);
  }

  deleteShare(): void {
    $.ajax({
      method: 'DELETE',
      url: `/api/share?key=${this.key}`,
      headers: {
        Authorization: "Bearer " + AuthService.token
      },
      success: (data) => {
        this.ondelete(this.key);
        this.modalRef.close();
      }
    })
  }
}
