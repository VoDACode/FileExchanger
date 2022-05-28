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
    })
  }

  ngOnInit(): void {
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
