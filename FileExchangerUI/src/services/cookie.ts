export class CookieService {
    public static get(name: string): string | undefined {
        let parts = document.cookie.split('; ');
        for (let i = 0; i < parts.length; i++) {
            let part = parts[i];
            let keyIndex = part.indexOf('=');
            let key = part.substring(0, keyIndex);
            if (key !== name)
                continue;
            return part.substring(keyIndex + 1, part.length);
        }
        return undefined;
    }
    public static set(name: string, value: string): void {
        document.cookie = `${name}=${value}`;
    }
}