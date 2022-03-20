import { Component, OnInit } from '@angular/core';
import { Account } from '../../models/account';
import { HttpService } from '../../services/http.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {

  account:Account = <Account>{};

  constructor(private http:HttpService) { }

  ngOnInit(): void {
    
  }

}
