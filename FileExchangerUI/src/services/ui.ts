import * as $ from 'jquery';

export class UIServise{
    private static isEnabled(service: string): boolean{
        let data = $.ajax({
            method: 'GET',
            async: false,
            url: `api/ui/service/${service}/enabled`
        });
        return data.responseText === 'true';
    }
    public static get isEnabledStorage(): boolean{
        return UIServise.isEnabled('storage');
    }
    public static get isEnabledExchanger(): boolean{
        return UIServise.isEnabled('exchaner');
    }
}