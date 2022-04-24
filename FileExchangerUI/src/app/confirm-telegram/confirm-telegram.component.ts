import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as $ from 'jquery';
import { AuthService } from 'src/services/auth';

@Component({
  selector: 'app-confirm-telegram',
  templateUrl: './confirm-telegram.component.html',
  styleUrls: ['./confirm-telegram.component.css']
})
export class ConfirmTelegramComponent implements OnInit, AfterViewInit {

  private code: string = "";
  public text: string = "";

  constructor(private activatedRoute: ActivatedRoute, private router: Router) {
    this.activatedRoute.queryParams.subscribe(p =>{
      if(!p.code){
        this.router.navigate(['/']);
        return;
      }
      this.code = p.code;
    });
  }
  ngAfterViewInit(): void {
    $.ajax({
      method: 'GET',
      url: `api/c/telegram?code=${this.code}`,
      headers:{
        Authorization: "Bearer " + AuthService.token
      },
      success: (data: any) => {
        this.router.navigate(['/my/user']);
      },
      error: (data: any) => {
        this.text = "ERROR<br>Reditect to '/'";
        setTimeout(() =>{
          this.router.navigate(['/']);
        }, 1500);
      }
    })
  }

  ngOnInit(): void {
  }

}
