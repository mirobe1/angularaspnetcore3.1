import { Injectable, OnDestroy } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { Subscription } from "rxjs";
import { AccountSubject } from "src/app/src/app/models/accountsubject";
import { HttpService } from "src/app/src/app/services/http.service";

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate, OnDestroy  {

    constructor(
        private router: Router,
        private http: HttpService
    ) {
      }

     ngOnDestroy() {
     } 
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const account = this.http.accountValue;
        console.log(account);
        if (account.Role) {

            if(route.url[0].path=='home')
            {
                if(account.Role == "Admin")
                {
                    this.router.navigate(['/admin']);
                    return false;
                }
                else{
                    this.router.navigate(['/account']);
                    return false;
                }
            }
            else
            {
                var role = route.data.role;

                if(role == account.Role){
                    return true;
                }else{
                    this.router.navigate(['/login']);
                    return false;
                }
                
            }
            // authorized so return true
            //return true;
        }

        // not logged in so redirect to login page with the return url 
        this.router.navigate(['/login']);
        return false;
    }
}