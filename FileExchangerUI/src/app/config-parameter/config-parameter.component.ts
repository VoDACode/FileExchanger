import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfigModule } from 'src/models/configModel';
import { EventModificationConfigParameter } from 'src/models/eventModificationConfigParameter';
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';
import { ChildHelper } from 'src/helpers/ChildHelper';

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
  public defaultValue(): void {

  }

  @ViewChild('box', { static: true })
  private box: ElementRef<HTMLDivElement> | undefined;

  public tag: any = null;
  @Input()
  public isRemove: boolean = false;
  @Input()
  public type: 'number' | 'text' | 'bool' | 'array' | 'object' = 'text';
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

  public get isObject(): boolean { return this.type == 'object'; }
  public get isArray(): boolean { return this.type == 'array'; }

  private _path: string = "";
  private _value: any = null;

  public get viewValue(): string {
    if (this.type == 'text') return `"${this.value}"`;
    return this.value;
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

  addNewObj(): void {
    let lastItem = Object.create(this.childrens[this.childrens.length - 1]);
    let newItem = new ConfigModule();
    newItem.show = true;
    newItem.type = lastItem.type;
    newItem.value = lastItem.value;
    newItem.root = lastItem.root;
    const copy = (from: ConfigModule, to: ConfigModule) => {
      for (let i = 0; i < from.childrens.length; i++) {
        let child = Object.create(from.childrens[i]);
        if(from.childrens[i].childrens.length == 0){
          to.childrens.push(child);
        }else{
          let tmp: ConfigModule = new ConfigModule();
          tmp.parameter = child.parameter;
          tmp.root = child.root;
          tmp.show = true;
          tmp.type = child.type;
          tmp.value = child.value;
          copy(child, tmp);
          to.childrens.push(tmp);
        }
      }
    }
    const clearVal = (item: ConfigModule) => {
      if (item.childrens.length === 0)
        return;
      for (let i = 0; i < item.childrens.length; i++) {
        if (item.childrens[i].type != 'array' && item.childrens[i].type != 'object') {
          item.childrens[i].defaultValue();
        } else {
          clearVal(item.childrens[i]);
        }
      }
    }
    copy(lastItem, newItem);
    clearVal(newItem);
    newItem.tag = '#new_item';
    let json = ConfigModule.toJson(newItem);
    newItem.parameter = (Number(lastItem.parameter) + 1).toString();
    $.ajax({
      method: 'POST',
      url: `api/config/add?p=${this._path}&value=${btoa(json)}`,
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      success: () =>{
        this.childrens.push(newItem);
      }
    })
  }

  deleteObj(): void{
    $.ajax({
      method: "DELETE",
      url: `api/config/delete?p=${this._path}`,
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      success: () => {
        if(!this.root)
          return;
        let index = this.root.childrens.findIndex(p => p.parameter == this.parameter && p.value == this.value && p.childrens == p.childrens);
        this.root.childrens.splice(index, 1);
      }
    })
  }

  changeEvent(event: any): void {
    if (this.type == 'bool') {
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
  }

  ngAfterViewInit(): void {
    if (!this.autoShow && this.param) {
      let params = this.param.split('.');
      let tPath = this._path.split('.');
      for (let i = 0; i < tPath.length; i++) {
        if (params[i] === tPath[i] && this.parameter === params[i]) {
          this.show = true;
          if (i == params.length - 1){
            this.blink();
          }
          break;
        }
      }
      this.autoShow = true;
    }
  }

  setShow(): void {
    this.show = !this.show;
  }

  copyPathName(): void {
    ChildHelper.copy(this._path);
  }

  blink(): void {
    //@ts-ignore
    let element = this.box.nativeElement;
    console.log(element);
    //@ts-ignore
    document.config_editor_box.scrollTo(0, element.offsetTop + element.offsetHeight + document.config_editor_box.offsetTop)
    $(element).animate({ borderWidth: '5px' }, 500, () => {
      setTimeout(() => {
        $(element).animate({ borderWidth: 0 }, 500);
      }, 750);
    });
  }

}
