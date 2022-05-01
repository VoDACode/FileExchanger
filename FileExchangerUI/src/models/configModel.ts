export class ConfigModule {
    public childrens: ConfigModule[] = [];
    public root: ConfigModule | undefined;
    public parameter: string = "";
    public value: any = "";
    public show: boolean = false;
    public type: 'number' | 'text' | 'bool' | 'array' | 'object' = 'text';
    public tag: any = null;
    public defaultValue(): void {
        switch (this.type) {
            case "text": this.value = ""; break;
            case "bool": this.value = false; break;
            case "number": this.value = 0; break;
            default: this.value = null; break;
        }
    }
    public static toJson(obj: ConfigModule): string { 
        const valToJson = (val: any): string => {
            if(Number.isFinite(val))
                return val.toString();
            return `"${val}"`;
        }
        const f = (item: ConfigModule, isLast: boolean = false, isArray: boolean = false): string => {
            let p = isArray || item.parameter.length == 0 ? '' : `"${item.parameter}":`;
            let result: string = `${p}${item.type == 'array' ? '[' : '{'}`;
            if(item.type == 'text'){
                result += `"${item.value}",`
            }else if(item.type == 'bool' || item.type == 'number'){
                result += `${item.value},`;
            }else if(item.type == 'object' || item.type == 'array'){
                for(let i = 0; i < item.childrens.length; i++){
                    if(item.childrens[i].childrens.length == 0){
                        result += `"${item.childrens[i].parameter}": ${valToJson(item.childrens[i].value)}`
                        if(i != item.childrens.length - 1)
                            result += ',';
                    }else{
                        result += f(item.childrens[i], i == item.childrens.length - 1, item.type == 'array');
                    }
                }
            }
            result += item.type == 'array' ? ']' : '}';
            result += isLast ? '' : ',';
            return result;
        }
        if(obj.parameter.length != 0)
            return '{' + f(obj, true) + '}';
        else
            return f(obj, true);
    }
}