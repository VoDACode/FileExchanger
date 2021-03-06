import { Component } from '@angular/core';
import { MdbModalRef } from 'mdb-angular-ui-kit/modal';
import {TranslateService} from "@ngx-translate/core";
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.user.list.html',
  styleUrls: ['./modal.user.list.css']
})
export class ModalUserList {
  users: Array<UserInListModel> = new Array<UserInListModel>();
  constructor(public modalRef: MdbModalRef<ModalUserList>, private translateService: TranslateService) {
    $.ajax({
      url:'api/group/info',
      headers:{
        Authorization: 'Bearer ' + AuthService.token
      },
      method:'GET',
      success: data => {
        this.users = data.users;
      },
      error: jqXHR => modalRef.close()
    });
  }

  kikUser(user: UserInListModel): void{
    if(user.lockData)
      return;
    $.ajax({
      url: `api/group/users/kik?uid=${user.id}`,
      headers:{
        Authorization: 'Bearer ' + AuthService.token
      },
      method: 'POST',
      success: data => {
        const index = this.users.indexOf(user);
        if (index > -1) {
          this.users.splice(index, 1);
        }
      }
    });
  }

  changePermissionAddUser(data: any, user: UserInListModel, index: number): void{
    if(user.lockData)
      return;
    $.ajax({
      url: `api/group/users/permissions/add-users?uid=${user.id}&m=${data.target.checked}`,
      headers:{
        Authorization: 'Bearer ' + AuthService.token
      },
      method: 'POST',
      success: res => {
        this.users[index].isPermissionAddUser = data.target.checked;
      }
    });
  }
}
class UserInListModel{
  lockData = false;
  id = "";
  isPermissionAddUser = false;
  joinDate = "";
  lastActive = "";
  title = "";
}
