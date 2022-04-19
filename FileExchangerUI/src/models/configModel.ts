export class ConfigModule{
    public childrens: ConfigModule[] = [];
    public root: ConfigModule | undefined;
    public parameter: string = "";
    public value: string = "";
    public show: boolean = false;
}