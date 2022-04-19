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
import { AuthComponent } from './auth/auth.component';
import { LoginComponent } from './login.component/login.component.component';
import { ReginComponent } from './regin/regin.component';
import { ConfirmEmailComponent } from './confirm.email/confirm.email.component';
import { HomePageComponent } from './home-page/home-page.component';
import { StartPageComponent } from './start-page/start-page.component';
import { StoragePageComponent } from './storage-page/storage-page.component';
import { StorageTreeComponent } from './storage-tree/storage-tree.component';
import { StorageContextMenuComponent } from './storage-context-menu/storage-context-menu.component';
import { ObjInStorageComponent } from './obj-in-storage/obj-in-storage.component';
import { ConfigEditorComponent } from './config-editor/config-editor.component';
import { ConfigParameterComponent } from './config-parameter/config-parameter.component';
import { SettingsWindowComponent } from './settings-window/settings-window.component';

export function HttpLoaderFactory(http: HttpClient): TranslateLoader {
  return new TranslateHttpLoader(http, './assets/locale/', '.json');
}

const appRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', component: StartPageComponent},
      { path: 'd/:key/:name', component: DownloadComponent },
      { path: 'auth', component: AuthComponent , children: [
        { path: '', component: LoginComponent},
        { path: 'login', component: LoginComponent},
        { path: 'regin', component: ReginComponent}
      ]},
      {
        path: 'join',
        children: [
          { path: ':key', component: JoinWorkingGroupComponent },
          { path: '', component: JoinWorkingGroupComponent }
        ]
      },
      {path: 'my', component: HomePageComponent, children: [
        {path: 'e', component: UploadComponent},
        {path: 's', component: StoragePageComponent},
        {path: 's/:dir', component: StoragePageComponent},
      ]},
      {path: 'config', children: [
        {path: '', component: ConfigEditorComponent},
        {path: ':key', component: ConfigEditorComponent}
      ]}
    ]
  }
]

@NgModule({
  declarations: [
    RootComponent, UploadComponent, DownloadComponent, FooterComponent,
    WorkingGroupComponent, ModalJoinInfo, ModalWorkingGroup, JoinWorkingGroupComponent, ModalUserList, AuthComponent, LoginComponent, ReginComponent,
    ConfirmEmailComponent,
    HomePageComponent,
    StartPageComponent,
    StoragePageComponent,
    StorageTreeComponent,
    StorageContextMenuComponent,
    ObjInStorageComponent,
    ConfigEditorComponent,
    ConfigParameterComponent,
    SettingsWindowComponent
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
export class AppModule { }
