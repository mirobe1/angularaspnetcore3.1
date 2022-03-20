import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountSignup } from '../../models/accountsignup';
import { AlertService } from '../../services/alert.service';
import { HttpService } from '../../services/http.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  account:AccountSignup = <AccountSignup>{};
  passwordsMatch:Boolean = true;
  constructor(private http:HttpService, private alert:AlertService, private router:Router) { }

  ngOnInit(): void {

  }

  CheckPasswords(){
    if(this.account.Password != this.account.RepeatPassword){
      this.passwordsMatch = false;
    }else{
      this.passwordsMatch = true;
    }
  }
  onSubmit(){
    if(this.account.Password != this.account.RepeatPassword){
      this.alert.error("Passwords do not match", {autoClose:true});
    }else{
      this.http.signup(this.account).subscribe(x => this.router.navigate(['/home']), err => {
        if(err.title){
          this.alert.error(err.title, {autoClose:true});
        }else{
          this.alert.error(err.message, {autoClose:true});
        }
       })
    }
  }

}
