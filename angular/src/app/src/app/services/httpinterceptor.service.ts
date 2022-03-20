import { HttpClient, HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class HttpinterceptorService implements HttpInterceptor {
  constructor(private http:HttpService, private router:Router, private httpcl:HttpClient) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      var account = this.http.accountValue;
      if(account && account.AccessToken != '' && !req.url.endsWith('login') && !req.url.endsWith('refresh-token') && !req.url.endsWith('signup')){
         req = req.clone({
          setHeaders: { Authorization: account.AccessToken }});
      

          return next.handle(req);

      }else{
        return next.handle(req);

      }


      
  }



}
