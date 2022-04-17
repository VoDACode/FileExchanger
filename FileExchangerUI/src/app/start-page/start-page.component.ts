import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth';
import * as $ from 'jquery';

@Component({
  selector: 'app-start-page',
  template: 'Redirect...',
})
export class StartPageComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
    if (!AuthService.isEnabledAuth()) {
      this.redirectMy();
      return;
    }
    if (AuthService.isAuth()) {
      this.redirectMy();
      return;
    } else {
      if(AuthService.getAuthorizationParameter() === "Always"){
        this.router.navigate(['/auth']);
      }else{
        this.redirectMy();
      }
    }
  }
  redirectMy() {
    $.ajax({
      method: 'GET',
      url: 'api/ui/default-service',
      success: (data) => {
        if (data === "FileExchanger") {
          this.router.navigate(['/my/e']);
        } else if (data === "FileStorage") {
          this.router.navigate(['/my/s']);
        }
      }
    })
  }
}
