import * as $ from 'jquery';
import { AuthService } from './auth';

export class DirectoryService {
    public static list(key: string, only_dir: boolean = false): any[] {
        let data = $.ajax({
            method: "GET",
            url: `api/files/s/${key}/list?${only_dir ? 'mode=only_dir' : ''}`,
            async: false,
            headers: {
                Authorization: "Bearer " + AuthService.token
            }
        });
        return data.responseJSON;
    }
    public static get getRootKey(): string {
        let data = $.ajax({
            method: "GET",
            url: `api/dir/s/get-root`,
            async: false,
            headers: {
                Authorization: "Bearer " + AuthService.token
            }
        });
        return data.responseText;
    }
}