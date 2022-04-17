import {Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css', '../../assets/css/content.css']
})
export class AuthComponent implements OnInit {
  constructor(private router: Router) { }
  ngOnInit(): void {
    if(!AuthService.isEnabledAuth()){
      this.router.navigate(['/']);
      return;
    }
    if(AuthService.isAuth()){
      this.router.navigate(['/my']);
    }
  }
}
