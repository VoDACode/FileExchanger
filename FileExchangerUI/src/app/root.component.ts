import { Router, NavigationEnd } from '@angular/router';
import {Component, OnInit} from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import * as $ from 'jquery';
import {environment} from "../environments/environment";
import {CookieService} from "ngx-cookie-service";
import { filter } from 'rxjs/operators';

declare const gtag: Function;

@Component({
  selector: 'app-root',
  templateUrl: 'root.component.html'
})
export class RootComponent implements OnInit {
  public getLocales(): string[]{
    return environment.locales;
  }
  selectLocale: string = environment.defaultLocale;
  constructor(private translateService: TranslateService, private cookie: CookieService, private router: Router) {
    $.ajax({
      url: 'api/user/create',
      method: 'POST',
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
  ngOnInit(): void {
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
}
