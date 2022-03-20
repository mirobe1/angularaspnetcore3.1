import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AccountSubject } from './src/app/models/accountsubject';
import { HttpService } from './src/app/services/http.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

  subscription : Subscription;
  account:AccountSubject = <AccountSubject>{};
  constructor(private http:HttpService, private router:Router) {
    this.subscription = this.http.account.subscribe(x => this.account = x);
   }
   ngOnInit() {
   }

   Logout(){
     this.http.logout();
   }

   ngOnDestroy() {
       this.subscription.unsubscribe();
   }
}
