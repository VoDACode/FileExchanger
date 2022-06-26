import { Component } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { MdbModalRef } from "mdb-angular-ui-kit/modal";
import { ObjInStorageModel } from "src/app/storage-page/storage-page.component";
import { AuthService } from "src/services/auth";
import * as $ from 'jquery';

@Component({
    selector: 'app-modal-open-file',
    templateUrl: './model.open.file.html',
    styleUrls: ['./model.open.file.css']
})
export class OpenFileModelComponent {
    item: ObjInStorageModel | undefined;
    constructor(public modalRef: MdbModalRef<OpenFileModelComponent>, private translateService: TranslateService) {

    }
    onDownload(): void {
        $.ajax({
            method: "GET",
            url: `api/files/s/get-disposable-key`,
            data: { DirectoryKey: this.item?.rootKey, FileKey: this.item?.key },
            headers: {
                Authorization: "Bearer " + AuthService.token
            },
            success: (data) => {
                //@ts-ignore
                DownloadFile(`api/files/s/download/${data.key}`);
            }
        });
    }
}