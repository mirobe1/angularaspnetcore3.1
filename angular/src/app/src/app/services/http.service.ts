import { HttpErrorResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { Account } from '../models/account';
import { map, catchError, take } from 'rxjs/operators';
import { AccountSubject } from '../models/accountsubject';
import { Router } from '@angular/router';
import { AccountSignup } from '../models/accountsignup';
import { environment } from 'src/environments/environment';
import { UploadEvent } from '../models/uploadevent';
import { RecipeReturn } from '../modules/user/models/recipereturn';
import { Recepies } from '../modules/user/models/recepies';
import { Recipe } from '../modules/user/models/recipe';
import { AccountReturn } from '../models/accountreturn';

const baseUrl = `${environment.apiUrl}`;

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private accountSubject: BehaviorSubject<AccountSubject>;
  public account: Observable<AccountSubject>;

  constructor(private http:HttpClient,private router: Router) { 
    this.accountSubject = new BehaviorSubject<AccountSubject>(<AccountSubject>{});
    this.account = this.accountSubject.asObservable();
  }

   httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

 httpPostOptions =
{   
    headers:
        new HttpHeaders (
        {
            "Content-Type": "application/json"
        }),
    withCredentials: true,
};

httpImageDownloadOptions =
{   
    headers:
        new HttpHeaders (
        {   
              "responseType": 'blob'
        })
};

  public get accountValue(): AccountSubject{
    return this.accountSubject.value;
  }

public setNextSubjectValue(as:AccountSubject){
  this.accountSubject.next(as);
}
  getAccounts(): Observable<AccountReturn[]>{
  
    return this.http.get<AccountReturn[]>(`${baseUrl}/accounts`).pipe(
      
    catchError(this.errorHandler));
    
    }

  signup(accSign:AccountSignup) : Observable<AccountSubject>{
    return this.http.post<any>(`${baseUrl}/accounts/signup`, accSign, { withCredentials: true }).pipe(
      map(accountSubject => {
        
        if(accountSubject.AccessToken){
            this.accountSubject.next(accountSubject);
            this.startRefreshTokenTimer(accountSubject.AccessTokenExpiryDate);
        }
        return accountSubject;
    })
    ,catchError(this.errorHandler));
  }

  login(UserName:string,Password:string): Observable<AccountSubject>{
  
    return this.http.post<any>(`${baseUrl}/accounts/login`, {UserName,Password}, { withCredentials: true }).pipe(
      map(accountSubject => {
          console.log(accountSubject);
        if(accountSubject.AccessToken){
            this.accountSubject.next(accountSubject);
            this.startRefreshTokenTimer(accountSubject.AccessTokenExpiryDate);
        }
        return accountSubject;
    })
    ,catchError(this.errorHandler));
    }

    logout() {
      this.http.post<any>(`${baseUrl}/accounts/logout` ,{}, { withCredentials: true }).subscribe();
      this.redirectToLoginPage()
    } 

    redirectToLoginPage(){
      this.accountSubject.next(<AccountSubject>{});
      this.stopRefreshTokenTimer();
      this.router.navigate(['/login']);
    }

    refreshToken(): Observable<AccountSubject>{
  
      return this.http.post<any>(`${baseUrl}/accounts/refresh-token`,{}, this.httpPostOptions).pipe(
        map(accountSubject => {
          if(accountSubject.AccessToken){
          this.accountSubject.next(accountSubject);
          this.startRefreshTokenTimer(accountSubject.AccessTokenExpiryDate);
          }
          return accountSubject;
      })
     // ,catchError(this.errorHandler)
      );
      }


      private refreshTokenTimeout:any;

      private startRefreshTokenTimer(accessTokenExpiryDate:Date) {
        var b = new Date();
        var timeout = (new Date(accessTokenExpiryDate).getTime()- b.getTime()) - 20000;
          this.refreshTokenTimeout = setTimeout(() => this.refreshToken().pipe(catchError(error=> {
            if (error.error  == "Refresh token expired") {
              this.redirectToLoginPage();              
            }
            return throwError(error.error || 'server Error');
          }
          )).subscribe(), timeout);
      }
  
      private stopRefreshTokenTimer() {
          clearTimeout(this.refreshTokenTimeout);
      }

      uploadImage(formData:FormData):Observable<any>{
        return this.http.post<any>(`${baseUrl}/Recepies/upload`, formData).pipe(
          catchError(this.errorHandler)
        )
      }
        // ,{reportProgress: true, observe: 'events' }

      getRecipesForAccount(accountId:number, recipeName:string, ingredientName:string, currentPage:number,pageSize:number): Observable<Recepies>{
        return this.http.get<Recepies>(`${baseUrl}/Recepies/recipes/${accountId}/${((recipeName == '')?'No': recipeName) }/${((ingredientName == '')?'No': ingredientName)}/${currentPage}/${pageSize}`).pipe(
      
          catchError(this.errorHandler));
      }

      getSingleImageForRecipe(accountId:number, recipeId:number): Observable<Blob>{
        return this.http.get(`${baseUrl}/Recepies/recipe-image/${accountId}/${recipeId}`, 
        {responseType: "blob"}
        ).pipe(
      
          catchError(this.errorHandler));
      }

      getRecipeForUpdate(recipeId:number): Observable<Recipe>{
        return this.http.get<Recipe>(`${baseUrl}/Recepies/recipe-for-update/${recipeId}`).pipe(
      
          catchError(this.errorHandler));
      }

      deleteRecipe(accountId:number, recipeId:number):Observable<any>{
        return this.http.delete<any>(`${baseUrl}/Recepies/recipe-delete/${accountId}/${recipeId}`).pipe(
          catchError(this.errorHandler)
        )
      }

      deleteAccount(accountId:number):Observable<any>{
        return this.http.delete<any>(`${baseUrl}/accounts/account-delete/${accountId}`).pipe(
          catchError(this.errorHandler)
        )
      }

  errorHandler = (error: HttpErrorResponse) => {
      //console.log(error);
       if(error.status == 401){
         this.logout();
       }
    return throwError(error.error || 'server Error');
     
   }
}
