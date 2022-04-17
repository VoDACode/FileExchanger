import * as $ from 'jquery';
import { AuthService } from './auth';

export class DirectoryService {
    // @ts-ignore
    public static list(key: string, only_dir: boolean = false): DirContentModel[] {
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
}