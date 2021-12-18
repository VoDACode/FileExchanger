import {Component} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import * as $ from 'jquery';
import {ConvertHelper} from "../../helpers/ConvertHelper";
import {Alert} from "../../models/Alert";
import {document} from "ngx-bootstrap/utils";
import {TranslateService} from "@ngx-translate/core";
import {AES} from "../../helpers/AES";
import {Md5} from "ts-md5/dist/md5";

@Component({
  selector: 'app-download',
  templateUrl: 'download.component.html'
})
export class DownloadComponent{
  public pin: string = "";
  public key: string = "";
  public name: string = "";
  public size: number = 0;
  public isLocked = false;
  public downloadPath: string = "";
  public alerts: Array<Alert> = new Array<Alert>();
  public UploadProgress(): number{
    return document.UploadProgress;
  }

  private pinHash(): string{
    const md5 = new Md5();
    return md5.appendStr(this.pin).end().toString();
  }

  constructor(private route: ActivatedRoute,private r: Router, private translateService: TranslateService) {
    document.UploadProgress = 0;
    route.params.subscribe(params => {
      this.key = params.key;
      this.name = params.name;
    });

    this.existFile({
      success: (data: any) => {
        this.size = data.size
        if(data.access == 1){
          this.downloadPath = `/api/files/download/${this.key}/${this.name}`;
          this.isLocked = false
          //this.download(this.key, this.name);
        }else {
          this.isLocked = true;
          this.downloadPath = `/api/files/download/${this.key}/${this.name}?p=`
        }
      },
      error: (jqXHR: any) => {
        this.r.navigate(['/']);
      }
    });
  }

  private existFile(res: any): void{
    $.ajax({
      url: `api/files/info/${this.key}/${this.name}`,
      method: 'GET',
      success: data => {
        if(res !== undefined)
        {
            res.success(data)
        }
      },
      error: jqXHR => {
        if(res !== undefined)
          {
            res.error(jqXHR)
          }
      }
    });
  }

  public downloadLockedFile(): void{
    $.ajax({
      url: `api/files/check/pin/${this.key}/${this.name}?p=${this.pinHash()}`,
      method: 'GET',
      success: data => {
        // this.isLocked = false;
        this.download(this.key, this.name, this.pinHash());
      },
      error: jqXHR => {
        this.alerts.push({
          type: 'danger',
          message: jqXHR.responseText
        });
      }
    })
  }

  public onDownload(): void{
    this.existFile({
      success: () => {
        $("#downloadForm").submit();
      },
      error: () => {
        this.r.navigate(['/']);
      }
    });
  }

  private download(key: string, name: string, password: string = ""): void{
    $.ajax({
      url: `/api/files/download/${key}/${name}${password == ""?"":`?p=${password}`}`,
      method: 'GET',
      xhrFields: {
        responseType: 'blob'
      },
      success: (data) => {
        AES.Decrypt(data, password, file => {
          const a = document.createElement('a');
          const url = window.URL.createObjectURL(file);
          a.href = url;
          a.download = name;
          document.body.append(a);
          a.click();
          a.remove();
          window.URL.revokeObjectURL(url);
        });
      },
      xhr: function()
      {
        let xhr = new window.XMLHttpRequest();
        //Download progress
        xhr.addEventListener("progress", function(evt){
          if (evt.lengthComputable) {
            let percentComplete = evt.loaded / evt.total;
            //Do something with download progress
            console.log(percentComplete);
          }
        });
        return xhr;
      }
    });
  }

  convertSize(size: number): string{
    return ConvertHelper.ConvertSize(size);
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }
}
