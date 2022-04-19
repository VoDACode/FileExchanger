import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfigModule } from 'src/models/configModel';
import { EventModificationConfigParameter } from 'src/models/eventModificationConfigParameter';
import * as $ from 'jquery';

@Component({
  selector: 'app-config-parameter',
  templateUrl: './config-parameter.component.html',
  styleUrls: ['./config-parameter.component.css']
})
export class ConfigParameterComponent implements OnInit, AfterViewInit, ConfigModule {

  private param: string = "";
  private autoShow: boolean = false;
  constructor(private r: ActivatedRoute) {
    this.r.params.subscribe(params => {
      if (!params)
        return;
      this.param = params.key;
    })
  }

  @ViewChild('box', { static: true })
  private box: ElementRef<HTMLDivElement> | undefined;

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
    if (this.getType == 'text') return `"${this.value}"`;
    return this.value;
  }

  public get getType(): 'number' | 'text' | 'bool' {
    if (Number.isInteger(this.value))
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
      if (!r) return;
      result = `${r.parameter}.${result}`;
      f(r.root);
    }
    f(this.root);
    result += this.parameter;
    return result;
  }

  changeEvent(event: any): void {
    if (this.getType === 'bool') {
      this.value = event.target.value === "true" ? true : false;
    } else if (!!(Number(event.target.value)) || event.target.value == "0") {
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

  ngAfterViewInit(): void {
    if (!this.autoShow && this.param) {
      let params = this.param.split('.');
      let tPath = this._path.split('.');
      for (let i = 0; i < tPath.length; i++) {
        if (params[i] === tPath[i] && this.parameter === params[i]) {
          this.show = true;
          if (i == params.length - 1)
            this.blink();
          break;
        }
      }
      this.autoShow = true;
    }
  }

  setShow(): void {
    this.show = !this.show;
  }

  blink(): void {
    //@ts-ignore
    let element = this.box.nativeElement;
    $(element).animate({ borderWidth: '5px' }, 500, () => {
      setTimeout(() => {
        $(element).animate({ borderWidth: 0 }, 500);
      }, 750);
    });
  }

}
