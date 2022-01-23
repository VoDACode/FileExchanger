import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RootComponent } from "./root.component";
import { RouterModule, Routes } from "@angular/router";
import { UploadComponent } from "./upload.component/upload.component";
import { DownloadComponent } from "./download.component/download.component";
import { FormsModule } from "@angular/forms";
import { FooterComponent } from "./footer.component/footer.component";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { WorkingGroupComponent } from "./working.group.component/working.group.component";
import { QrCodeModule } from 'ng-qrcode';
import { ModalJoinInfo } from './modals/modal/modal.join-info';
import { MdbModalModule } from 'mdb-angular-ui-kit/modal';
import { ModalWorkingGroup } from './modals/modal/modal.working-group';
import { JoinWorkingGroupComponent } from "./join.working.group.component/join.working.group.component";
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { CookieService } from "ngx-cookie-service";
import { ModalUserList } from "./modals/modal/modal.user.list";
import { DragDropModule } from "@angular/cdk/drag-drop";

export function HttpLoaderFactory(http: HttpClient): TranslateLoader {
  return new TranslateHttpLoader(http, './assets/locale/', '.json');
}

const appRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', component: UploadComponent },
      { path: 'd/:key/:name', component: DownloadComponent },
      {
        path: 'join',
        children: [
          { path: ':key', component: JoinWorkingGroupComponent },
          { path: '', component: JoinWorkingGroupComponent }
        ]
      }
    ]
  }
]

@NgModule({
  declarations: [
    RootComponent, UploadComponent, DownloadComponent, FooterComponent,
    WorkingGroupComponent, ModalJoinInfo, ModalWorkingGroup, JoinWorkingGroupComponent, ModalUserList
  ],
  imports: [
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      useDefaultLang: false,
    }),
    BrowserModule,
    RouterModule.forRoot(appRoutes),
    FormsModule,
    NgbModule,
    QrCodeModule,
    MdbModalModule,
    HttpClientModule,
    DragDropModule
  ],
  exports: [TranslateModule],
  providers: [CookieService],
  bootstrap: [RootComponent]
})
export class AppModule {}
