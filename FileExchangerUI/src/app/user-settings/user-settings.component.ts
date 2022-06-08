import { AfterViewInit, Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as $ from 'jquery'
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit, AfterViewInit{

  public username: string = "";
  public email: string = "";
  public oldPassword: string = "";
  public newPassword: string = "";
  public createdTelegram = false;

  constructor(private translate: TranslateService) { }

  ngAfterViewInit(): void {
    $.ajax({
      method: 'GET',
      url: 'api/telegram/enable',
      headers: {
        Authorization: "Bearer " + AuthService.token
      },
      success: data =>{
        this.createdTelegram = !data;
      }
    });
    $.ajax({
      method: 'GET',
      url: 'api/user/my',
      headers: {
        Authorization: "Bearer " + AuthService.token
      },
      success: (data) => {
        this.username = data.authClient.username;
        this.email = data.authClient.email;
      }
    });
  }

  ngOnInit(): void {
  }

  saveChanges(): void {
    $.ajax({
      method: 'PUT',
      url: 'api/user/my',
      headers:{
        Authorization: "Bearer " + AuthService.token,
        email: this.email,
        username: this.username,
        newPassword: this.newPassword,
        oldPassword: this.oldPassword
      }
    })
  }

  openTg(): void {
    $.ajax({
      method: 'GET',
      url: 'api/telegram/info',
      success: data => {
        document.location = data;
      }
    })
  }

  deleteTelegram(): void {
    $.ajax({
      method: 'DELETE',
      url: 'api/telegram/delete',
      success: data => {
        this.createdTelegram = true;
      }
    })
  }

}
