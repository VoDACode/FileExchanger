import { Component} from '@angular/core';
import {WorkingGroupComponent} from "../../working.group.component/working.group.component";
import {MdbModalRef, MdbModalService} from "mdb-angular-ui-kit/modal";
import {ModalJoinInfo} from "../join-info/modal.join-info";
import {TranslateService} from "@ngx-translate/core";
import {CookieService} from "ngx-cookie-service";

@Component({
  selector: 'app-modal',
  templateUrl: './modal.working-group.html',
  styleUrls: ['./modal.working-group.css']
})
export class ModalWorkingGroup extends WorkingGroupComponent {
  constructor(modalService: MdbModalService, public modalRef: MdbModalRef<ModalJoinInfo>,
              translateService: TranslateService, cookieService: CookieService) {
    super(modalService, translateService, cookieService);
  }

}
