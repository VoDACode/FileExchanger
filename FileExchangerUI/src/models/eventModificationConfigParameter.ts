import { ConfigModule } from "./configModel";

export class EventModificationConfigParameter{
    value: any;
    path: string = "";
    lastValue: any;
    target: ConfigModule;
    public constructor(value: any, lastValue: any, path: string, target: ConfigModule){
        this.value = value;
        this.lastValue = lastValue;
        this.path = path;
        this.target = target;
    }
}