import { Component, OnInit } from '@angular/core';
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket'
import { CookieService } from 'src/services/cookie';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-via-telegram',
  templateUrl: './login-via-telegram.component.html',
  styleUrls: ['./login-via-telegram.component.css']
})
export class LoginViaTelegramComponent implements OnInit {

  constructor(private router: Router) { }

  email: string = "";
  error: string = "";
  private mode: 'wait' | 'ok' | 'fail' | 'none' = 'none';
  public get isWaiting():boolean { return this.mode === 'wait'}
  public get isNone():boolean { return this.mode === 'none'}
  public get isError():boolean { return this.mode === 'fail'}
  ngOnInit(): void {
  }
  onLogin(): void {
    $.ajax({
      method: 'POST',
      url: `api/telegram/login?e=${this.email}`,
      success: (data) =>{
        const subject =  webSocket(`wss://${location.host}/ws/auth?c=${data}`);
        this.mode = 'wait';
        subject.subscribe((data: any) =>{
          if(data.type === "ok"){
            CookieService.set('auth_token', data.data);
            this.mode = 'ok';
            this.router.navigate(['/my']);
            document.location.reload();
            return;
          }else if(data.type === "error"){
            this.mode = 'fail';
            this.error = data.data;
            this.router.navigate(['/']);
            return;
          }else if(data.type === "fail"){
            this.error = data.data;
            this.mode = 'fail';
            this.router.navigate(['/auth']);
            return;
          }
        })
      }
    })
  }
}
