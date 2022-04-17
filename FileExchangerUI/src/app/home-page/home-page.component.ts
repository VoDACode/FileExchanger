import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {

  constructor(private router: Router, private action: ActivatedRoute) {
    if(location.pathname == "/my"){
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

  ngOnInit(): void {
    if(AuthService.getAuthorizationParameter() == "Always" && !AuthService.isAuth()){
      this.router.navigate(['/']);
      return;
    }
  }

}
