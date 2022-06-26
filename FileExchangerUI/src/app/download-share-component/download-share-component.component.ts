import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-download-share-component',
  template: 'Downloading'
})
export class DownloadShareComponent implements OnInit {

  key: string = "";
  constructor(private route: ActivatedRoute,private r: Router, private translateService: TranslateService) { 
    route.params.subscribe(params => {
      this.key = params.key;
    })
  }

  ngOnInit(): void {
    //@ts-ignore
    DownloadFile(`${location.origin}/api/share/${this.key}`);
    // window.close();
  }

}
