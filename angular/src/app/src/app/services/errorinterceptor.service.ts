import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class ErrorinterceptorService implements HttpInterceptor {

  constructor(private http:HttpService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(err => {
        if ([401, 403].includes(err.status) && this.http.accountValue) {
            // auto logout if 401 or 403 response returned from api
            this.http.logout();
        }

        const error = (err && err.error && err.error.message) || err.statusText;
        console.log(err);
        return throwError(err);
    }))
}
}
