import { Component, OnInit } from '@angular/core';
import { AccountReturn } from '../../models/accountreturn';
import { HttpService } from '../../services/http.service';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  accounts:AccountReturn[] = []

  constructor(private http:HttpService, private spinner:SpinnerService) { }
  
  ngOnInit(): void {
    this.GetAccounts()
  }
  GetAccounts(){
    
    this.spinner.Spinner(true);
    this.http.getAccounts().subscribe(x =>  
      {
        this.accounts = x; 
        this.spinner.Spinner(false);
        
      }
       , err => {
         console.log(err);
         this.spinner.Spinner(false);
        });

  }

  DeleteAccount(accountId:number){
    this.spinner.Spinner(true);
    this.http.deleteAccount(accountId).subscribe(
      x => { this.GetAccounts()},
      err => {console.log(err); this.spinner.Spinner(false);}
    )
  }

}
