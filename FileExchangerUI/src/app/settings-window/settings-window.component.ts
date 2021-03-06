import { AfterViewInit, Component, EventEmitter, HostBinding, HostListener, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'src/services/auth';
import { UIServise } from 'src/services/ui';

@Component({
  selector: 'app-settings-window',
  templateUrl: './settings-window.component.html',
  styleUrls: ['./settings-window.component.css']
})
export class SettingsWindowComponent implements OnInit, AfterViewInit {

  public isOpened = false;
  public get isAdmin(): boolean {
    return AuthService.isAdmin;
  }
  public get isEnabledStorage(): boolean {
    return UIServise.isEnabledStorage;
  }
  public get isEnabledExchanger(): boolean {
    return UIServise.isEnabledExchanger;
  }
  public get logined(): boolean {
    return AuthService.isAuth();
  }

  constructor(private translateService: TranslateService, private router: Router) { }
  ngAfterViewInit(): void {
    //@ts-ignore
    document.onclickToBg = this.close.bind(this);
  }

  ngOnInit(): void {
  }

  open(): void {
    this.isOpened = !this.isOpened;
    if (this.isOpened)
      //@ts-ignore
      document.showBg();
  }

  close(): void {
    this.isOpened = false;
    console.log(this);
  }

  onLogout(): void {
    AuthService.logout();
    this.router.navigate(['/']);
    document.location.reload();
  }
}
