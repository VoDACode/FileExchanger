import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import * as $ from "jquery";
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-login.component',
  templateUrl: './login.component.component.html',
  styleUrls: ['./login.component.component.css', '../../assets/css/content.css']
})
export class LoginComponent implements OnInit {
  email: string = "";
  password: string = "";
  isViewRegin = false;
  isViewTelegram = false;
  constructor(private router: Router, private cookie: CookieService, private translate: TranslateService) { }
  ngOnInit(): void {
    $.ajax({
      method: 'GET',
      url: '/api/ui/auth/accounts/enable',
      success: (data) => {
        if(!data){ 
          this.router.navigate(['/']);
        }
      }
    })
    $.ajax({
      method: 'GET',
      url: '/api/ui/auth/accounts/enable-registration',
      success: (data) => {
        this.isViewRegin = data;
      }
    });
    $.ajax({
      method: 'GET',
      url: '/api/ui/auth/accounts/enable-telegram',
      success: (data) => {
        this.isViewTelegram = data;
      }
    });
  }

  onLogin(){
    $.ajax({
      method: 'POST',
      url: `/api/auth/login`,
      dataType: 'json',
      contentType: 'application/json',
      data: JSON.stringify({
        email: this.email,
        password: this.password
      }),
      error: (data) => {
        alert(data.responseText);
      },
      success: (data) => {
        this.router.navigate(['/my']);
        document.location.reload();
      }
    })
  }
}
