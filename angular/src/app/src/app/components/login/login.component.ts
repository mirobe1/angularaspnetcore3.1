import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertService } from '../../services/alert.service';
import { HttpService } from '../../services/http.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  UserName:string="";
  Password:string = "";
  constructor(private http:HttpService, private router:Router, private alert:AlertService) { }

  ngOnInit(): void {
  }

  onSubmit(){
    this.http.login(this.UserName, this.Password).subscribe(x => { 
      console.log(x);
      this.router.navigate(['/home'])
    },err => {
      this.alert.error(err.message,{autoClose:true});
    });
  }
}
