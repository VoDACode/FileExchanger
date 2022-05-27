import { Component } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { MdbModalRef } from "mdb-angular-ui-kit/modal";
import { ObjInStorageModel } from "src/app/storage-page/storage-page.component";

@Component({
    selector: 'app-modal-storage-object-properties',
    styleUrls: ['./modal.storageObjectProperties.css'],
    templateUrl: './modal.storageObjectProperties.html'
})
export class ModalStorageObjectProperties{
    public item: ObjInStorageModel | undefined;
    constructor(public modalRef: MdbModalRef<ModalStorageObjectProperties>, private translate: TranslateService){

    }
}