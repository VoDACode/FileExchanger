import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ConfigModule } from 'src/models/configModel';
import { EventModificationConfigParameter } from 'src/models/eventModificationConfigParameter';

@Component({
  selector: 'app-config-parameter',
  templateUrl: './config-parameter.component.html',
  styleUrls: ['./config-parameter.component.css']
})
export class ConfigParameterComponent implements OnInit, ConfigModule {

  constructor() { }
  @Input()
  public childrens: ConfigModule[] = [];
  @Input()
  public root: ConfigModule | undefined;
  @Input()
  public parameter: string = "";
  @Input()
  public value: any = null;
  @Input()
  public show: boolean = false;
  @Output()
  public onchangeParameter: EventEmitter<EventModificationConfigParameter> = new EventEmitter();

  private _path: string = "";
  private _value: any = null;
  public _type: string = "";

  public get viewValue(): string {
    if(this.getType == 'text') return `"${this.value}"`;
    return this.value;
  }

  public get getType(): 'number' | 'text' | 'bool'{
    if(Number.isInteger(this.value))
      return 'number';
    if (this.value === true || this.value === false || this.value.toLowerCase() == 'true' || this.value.toLowerCase() == 'false')
      return 'bool';
    return 'text';
  }
  public get isObject(): boolean {
    return this.childrens.length > 0
  }

  public get path(): string {
    let result = '';
    const f = (r: ConfigModule | undefined) => {
      if(!r) return;
      result = `${r.parameter}.${result}`;
      f(r.root);
    }
    f(this.root);
    result += this.parameter;
    return result;
  }

  changeEvent(event: any): void {
    if(this.getType === 'bool'){
      this.value = event.target.value === "true" ? true : false;
    }else if(!!(Number(event.target.value)) || event.target.value == "0"){
      this.value = Number(event.target.value);
    }
    this.onchangeParameter.emit(new EventModificationConfigParameter(this.value, this._value, this._path, this));
  }

  change(event: EventModificationConfigParameter): void {
    this.onchangeParameter.emit(event);
  }

  ngOnInit(): void {
    this._path = this.path;
    this._value = this.value;
    this._type = this.getType;
  }

  setShow(): void {
    this.show = !this.show;
  }

}
