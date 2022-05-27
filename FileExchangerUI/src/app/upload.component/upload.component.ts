import { Component } from "@angular/core";
import * as $ from 'jquery';
import { FileModel } from "../../models/FileModel";
import { document } from "ngx-bootstrap/utils";
import { ConvertHelper } from "../../helpers/ConvertHelper";
import { Alert } from "src/models/Alert";
import { MdbModalRef, MdbModalService } from "mdb-angular-ui-kit/modal";
import { ModalJoinInfo } from "../modals/modal/modal.join-info";
import { ModalWorkingGroup } from "../modals/modal/modal.working-group";
import { TranslateService } from "@ngx-translate/core";
import { Md5 } from 'ts-md5/dist/md5';
import * as CryptoJS from 'crypto-js';
import { AES } from "../../helpers/AES";
import { SaveTimePattern } from "src/models/SaveTimePatternModel";
import { AuthService } from "src/services/auth";
import { Router } from "@angular/router";
import { ChildHelper } from "src/helpers/ChildHelper";

@Component({
  selector: 'app-upload',
  templateUrl: 'upload.component.html',
  styleUrls: ['upload.component.css']
})
export class UploadComponent {
  public modalRef: MdbModalRef<ModalJoinInfo> | undefined;
  public files: Array<FileModel> = new Array<FileModel>();
  public isLimited: boolean = false;
  public pin: string = "";
  public MaxCountDownload: number = 0;
  public SaveTime: number = 0;
  public alerts: Array<Alert> = new Array<Alert>();
  public downloadPath: string = "";
  public saveTimePatterns: SaveTimePattern[] = [];
  public GetSavePatternUnit(item : SaveTimePattern): string{
    return this.translateService.instant(`Upload.SaveTime.${item.unit}`);
  }
  public UploadProgress(): number {
    return document.UploadProgress;
  }
  private file(): any {
    // @ts-ignore
    return document.getElementById('selectFile').files[0]
  }

  constructor(private modalService: MdbModalService, private translateService: TranslateService, private router: Router) {
    $.ajax({
      method: 'GET',
      url: 'api/ui/auth/exchnge/use',
      success: (data: any) =>{
        if(data && !AuthService.isAuth()){
          this.router.navigate(['/auth']);
        }
      }
    });
    document.UploadProgress = 0;
    document.UpdateFileList = () => {
      $.ajax({
        url: 'api/files/e/list',
        method: 'GET',
        headers:{
          Authorization: 'Bearer ' + AuthService.token
        },
        success: data => {
          for (const dataKey in data) {
            let obj = data[dataKey];
            this.files.push(obj);
          }
        }
      })
    }
    document.UpdateFileList();
    this.getSaveTimePatterns();
  }

  private isOk(): boolean {
    if (this.isLimited && this.pin.length == 0)
      return false;
    if (this.file() === undefined)
      return false;
    return this.SaveTime !== 0 && this.MaxCountDownload !== 0;
  }

  onSelectPassword(p: any): void {
    this.translateService.get("Warnings.Password").subscribe(translate => {
      if (p.target.checked)
        alert(translate);
    });
  }

  onCopyLink(obj: FileModel): void {
    ChildHelper.copy(`${location.protocol}//${location.host}/d/${obj.downloadKey}/${obj.name}`);
  }

  onUpload(): void {
    if (!this.isOk() || !this.chekLimit())
      return;
    const fd = new FormData();
    const md5 = new Md5();
    this.pin = md5.appendStr(this.pin).end().toString();
    const url = `api/files/e/upload?m=${this.isLimited ? `2&p=${this.pin}` : '1'}&st=${this.SaveTime}&dc=${this.MaxCountDownload}`;
    if (this.isLimited) {
      AES.Encrypt(this.file(), this.pin, (file) => {
        fd.append('file', file);
        this.upload(fd, url);
      });
    } else {
      fd.append('file', this.file());
      console.log(fd);
      this.upload(fd, url);
    }
    this.pin = "";
    document.getElementById('selectFile').value = "";
    this.SaveTime = 0;
    this.MaxCountDownload = 0;
  }

  private upload(fd: FormData, url: string): void {
    $.ajax({
      url: url,
      type: 'POST',
      data: fd,
      headers: {
        Accept: 'application/json',
        Authorization: 'Bearer ' + AuthService.token
      },
      contentType: false,
      processData: false,
      success: (response) => {
        if (response !== 0) {
          this.files.push(response);
          document.UploadProgress = 0;
        }
      },
      error: jqXHR => {
        this.alerts.push({
          type: 'danger',
          message: jqXHR.responseText
        });
      },
      xhr: function () {
        var xhr = new window.XMLHttpRequest();

        // Upload progress
        xhr.upload.addEventListener("progress", function (evt) {
          if (evt.lengthComputable) {
            let percentComplete = evt.loaded / evt.total;
            percentComplete = Number((percentComplete * 100).toFixed(1));
            if (percentComplete >= 99.999) {
              document.UploadProgress = "Processing...";
            } else {
              document.UploadProgress = percentComplete;
            }
          }
        }, false);

        return xhr;
      }
    })
  }

  onDownload(obj: FileModel): void {
    $('#downloadForm_' + obj.id).submit();
  }

  private chekLimit(): boolean {
    const url = `api/files/e/check/limit?fs=${this.file().size}`;
    let res = $.ajax({
      url: url,
      async: false,
      method: 'GET'
    });
    if (res.status !== 200) {
      this.alerts.push({
        type: 'warning',
        message: res.responseText
      });
      return false;
    }
    return true;
  }

  onDelete(obj: FileModel): void {
    const url = `api/files/e/delete/${obj.downloadKey}/${obj.name}`;
    $.ajax({
      url: url,
      method: 'POST',
      headers:{
        Authorization: 'Bearer ' + AuthService.token
      },
      success: data => {
        const index = this.files.indexOf(obj);
        if (index > -1) {
          this.files.splice(index, 1);
        }
      },
      error: jqXHR => {
        this.alerts.push({
          type: 'warning',
          message: jqXHR.responseText
        });
      }
    })
  }

  getTime(time: string): string {
    return new Date(time).toLocaleString();
  }

  viewDeleteTime(obj: FileModel): string {
    let createDate = new Date(obj.createDate);
    createDate.setSeconds(createDate.getSeconds() + Number(obj.saveTime))
    return createDate.toLocaleString();
  }

  convertSize(size: number): string {
    return ConvertHelper.ConvertSize(size);
  }

  setLengthName(str: string): string {
    let res = "";
    let stopIndex = str.length < 15 ? str.length : 15;
    for (let i = 0; i < stopIndex; i++) {
      res += str[i];
    }
    return (res.length != str.length) ? `${res}...` : res;
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }

  openGroupMenu(): void {
    this.modalRef = this.modalService.open(ModalWorkingGroup, {
      data: {
        isModal: true
      }
    });
  }

  getSaveTimePatterns(): void {
    $.ajax({
      method: "GET",
      async: true,
      url: "/api/ui/save-patterns",
      success: (data) => {
        for (let i = 0; i < data.length; i++) {
          this.saveTimePatterns.push(new SaveTimePattern(data[i].unit.toUpperCase(), data[i].value));
        }
      }
    })
  }
}
