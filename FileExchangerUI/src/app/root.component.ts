import { Router, NavigationEnd } from '@angular/router';
import {AfterViewInit, Component, EventEmitter, OnInit, Output} from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import * as $ from 'jquery';
import {environment} from "../environments/environment";
import {CookieService} from "ngx-cookie-service";
import { filter } from 'rxjs/operators';
import { AuthService } from 'src/services/auth';

declare const gtag: Function;

@Component({
  selector: 'app-root',
  templateUrl: 'root.component.html',
  styleUrls: ['./root.component.css']
})
export class RootComponent implements OnInit, AfterViewInit {
  public getLocales(): string[]{
    return environment.locales;
  }
  isViewAuth = false;
  showBg = false;
  selectLocale: string = environment.defaultLocale;
  constructor(private translateService: TranslateService, private cookie: CookieService, private router: Router) {
    $.ajax({
      url: 'api/user/create',
      method: 'POST',
      headers: {
        Authorization: 'Bearer ' + AuthService.token
      },
      async: false
    })
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
      //@ts-ignore
    ).subscribe((event: NavigationEnd) => {
      /** START : Code to Track Page View  */
      console.log("EVENT CLICK", event.urlAfterRedirects);
      gtag('event', 'page_view', {
        page_path: event.urlAfterRedirects
      })
      /** END */
    })
  }
  ngAfterViewInit(): void {
    //@ts-ignore
    document.showBg = (() => {
      this.showBg = true;
    }).bind(this);
  }
  ngOnInit(): void {
    $.ajax({
      method: 'GET',
      url: 'api/ui/auth/accounts/enable',
      success: (data) => {
        this.isViewAuth = data;
      }
    })
    if(AuthService.isAuth()){
      this.isViewAuth = false;
    }
    this.selectLocale = this.cookie.get('locale');
    if(!this.selectLocale){
      this.selectLocale = environment.defaultLocale;
      this.cookie.set('locale', this.selectLocale)
    }
    this.translateService.use(this.selectLocale);
  }

  onChangeLocal(): void{
    this.translateService.use(this.selectLocale);
    this.cookie.set('locale', this.selectLocale);
  }

  clickToBg(): void{
    //@ts-ignore
    if(document.onclickToBg)
    //@ts-ignore
      document.onclickToBg();
      this.showBg = false;
  }
}
