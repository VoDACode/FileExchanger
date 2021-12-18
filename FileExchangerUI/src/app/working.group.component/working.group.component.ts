import {Component} from "@angular/core";
import * as $ from 'jquery';
import {UserModel} from "../../models/UserModel";
import {Alert} from "../../models/Alert";
import { MdbModalRef, MdbModalService } from "mdb-angular-ui-kit/modal";
import {ModalJoinInfo} from "../modals/modal/modal.join-info";
import {TranslateService} from "@ngx-translate/core";
import {ModalUserList} from "../modals/modal/modal.user.list";
import {CookieService} from "ngx-cookie-service";

@Component({
  selector:'working-group',
  templateUrl: 'working.group.component.html',
  styleUrls: ['working.group.component.css']
})
export class WorkingGroupComponent{
  // @ts-ignore
  public modalRef: MdbModalRef<ModalJoinInfo>;
  public alerts: Array<Alert> = new Array<Alert>();
  public isPermissionAddUser: boolean = false;
  public isExistInGroup: boolean = false;
  public joinKey: string = "";
  public joinToKey: string = "";
  public isCreator: boolean = false;
  public users: Array<any> = new Array<any>();
  public UserInGroup: UserModel = new UserModel();
  public sessionName: string = "";
  public getJoinUrl(): string{
    return `http://${location.host}/join/${this.joinKey}`;
  }

  constructor(private modalService: MdbModalService, private translateService: TranslateService,
              private cookieService: CookieService) {
    this.getInfo();
    this.sessionName = this.cookieService.get('sessionName');
  }

  private getInfo(): void{
    $.ajax({
      url: 'api/group/info',
      method: 'GET',
      success: data => {
        if(data.group.key !== undefined) {
          this.joinKey = data.group.key
          this.isPermissionAddUser = true;
        }
        if(data.users !== undefined){
          this.joinKey = data.group.key
          this.isPermissionAddUser = true;
          this.isCreator = true;
          this.users = data.users
        }
        this.isExistInGroup = true;
      },
      error: jqXHR => {
        this.isExistInGroup = false;
      }
    });
  }

  onSetSessionName(): void{
    if(this.sessionName.length === 0) {
      this.alerts.push({
        message: 'Please fill "Session name"!',
        type: 'danger'
      })
      return;
    }
    $.ajax({
      url:`api/group/set/session-name?n=${this.sessionName}`,
      method: 'POST'
    });
    this.cookieService.set('sessionName', this.sessionName);
  }

  onCreate(): void{
    if(this.sessionName.length === 0) {
      this.alerts.push({
        message: 'Please fill "Session name"!',
        type: 'danger'
      })
      return;
    }
    this.cookieService.set('sessionName', this.sessionName);
    let url = `api/group/create?userName=${this.sessionName}`
    $.ajax({
      url: url,
      method: 'POST',
      error: jqXHR => {
        this.alerts.push({
          type: 'danger',
          message: jqXHR.responseText
        });
      },
      success: data => {
        this.isExistInGroup = true;
        this.sessionName = data.user.title;
        this.isCreator = true;
        this.isPermissionAddUser = true;
        this.joinKey = data.key;
      }
    })
  }

  onLeave(): void{
    let res = prompt('Are you sure? Enter "yes" to confirm.');
    if(res === null || res.toLowerCase() != 'yes')
      return;
    $.ajax({
      url: 'api/group/remove',
      method: 'POST',
      error: jqXHR => {
        this.alerts.push({
          message: jqXHR.responseText,
          type: 'warning'
        });
      },
      success: data => {
        this.isExistInGroup = false;
        this.joinKey = "";
        this.users.length = 0;
      }
    })
  }

  onJoinToKey(): void{
    let url = `/api/group/join/${this.joinToKey}`;
    console.log(url);
    console.log(this.joinToKey);
    $.ajax({
      url: url,
      method: 'POST',
      success: data => {
        this.getInfo();
        // @ts-ignore
        document.UpdateFileList();
      },
      error: jqXHR => {
        this.alerts.push({
          type: 'warning',
          message: jqXHR.responseText
        });
      }
    });
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }

  viewJoinInfo(): void{
    this.modalRef = this.modalService.open(ModalJoinInfo, { data: {
        key: this.joinKey,
        isCreator: this.isCreator
    }});
  }

  viewUserList(): void{
    this.modalRef = this.modalService.open(ModalUserList, {
      modalClass: 'modal-user-list'
    });
  }
}
