import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as $ from "jquery";
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-regin',
  templateUrl: './regin.component.html',
  styleUrls: ['./regin.component.css', '../../assets/css/content.css']
})
export class ReginComponent implements OnInit {
  username: string = "";
  email: string = "";
  password: string = "";
  constructor(private router: Router) { }

  ngOnInit(): void {
    if(AuthService.isAuth()){
      this.router.navigate(['/my']);
      return;
    }
    $.ajax({
      method: 'GET',
      url: '/api/ui/auth/accounts/enable',
      success: (data) => {
        if(!data){ 
          this.router.navigate(['/']);
        }
      }
    })
    $.ajax({
      method: 'GET',
      url: '/api/ui/auth/accounts/enable-registration',
      success: (data) => {
        if(!data)
          this.router.navigate(['/']);
      }
    });
  }
  onRegister(): void {
   $.ajax({
     method: 'POST',
     url: `/api/auth/regin?e=${this.email}&p=${this.password}&n=${this.username}`,
     success: (data) => {
      this.router.navigate(['/confirm']);
     },
     error: (data) => {
      alert(data.responseText);
     }
   }) 
  }
}
