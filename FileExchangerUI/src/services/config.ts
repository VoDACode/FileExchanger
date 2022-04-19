import * as $ from 'jquery';
import { AuthService } from './auth';

export class ConfigService {
    public static get getConfig(): any {
        let data = $.ajax({
            method: 'GET',
            url: 'api/config',
            async: false,
            headers: { Authorization: 'Bearer ' + AuthService.token }
        });
        return data.responseText;
    }
}