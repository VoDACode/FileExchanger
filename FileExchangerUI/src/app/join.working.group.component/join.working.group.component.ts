import {Component} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import * as $ from 'jquery';

@Component({
  template: 'Redirect...',
  selector: 'app-join-to-group'
})
export class JoinWorkingGroupComponent{
  key: string = "";
  constructor(private route: ActivatedRoute,private r: Router) {
    route.params.subscribe(params => {
      if(!params.key)
      {
        r.navigate(['/']);
      }
      this.key = params.key;
      this.join();
    });
  }
  private join(): void{
    let url = `/api/group/join/${this.key}`;
    $.ajax({
      url: url,
      method: 'POST',
      success: data => {
        this.r.navigate(['/']);
      },
      error: jqXHR => {
        this.r.navigate(['/']);
      }
    });
  }
}
