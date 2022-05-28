import * as $ from 'jquery';
import { CookieService } from './cookie';

export class AuthService {
    public static get token(): string | undefined {
        return CookieService.get("auth_token");
    }
    public static logout(): void {
        CookieService.delete("auth_token");
        CookieService.delete("u_key");
    }

    public static isAuth(): boolean {
        let data = $.ajax({
            method: 'GET',
            url: 'api/auth/check',
            headers: {
                Accept: 'application/json',
                Authorization: 'Bearer ' + this.token
            },
            async: false,
        });
        return data.status === 200
    }
    public static isEnabledAuth(): boolean {
        let data = $.ajax({
            method: 'GET',
            url: '/api/ui/auth/accounts/enable',
            async: false
        });
        return data.responseText == "true";
    }
    public static getAuthorizationParameter(): "Always" | "Never" | "Optionally" {
        let data = $.ajax({
            method: 'GET',
            url: '/api/ui/auth/accounts/authorization',
            async: false
        });
        //@ts-ignore
        return data.responseText;
    }
    public static get isAdmin(): boolean {
        let data = $.ajax({
            method: 'GET',
            url: '/api/user/is-admin',
            async: false,
            headers: {
                Accept: 'application/json',
                Authorization: 'Bearer ' + this.token
            }
        });
        return data.status === 200 && data.responseText === 'true';
    }
}