import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS }    from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { AccountComponent } from './src/app/components/account/account.component';
import { HomeComponent } from './src/app/components/home/home.component';
import { AuthGuard } from 'src/helpers/auth.guard';
import { AdminComponent } from './src/app/components/admin/admin.component';
import { LoginComponent } from './src/app/components/login/login.component';
import { HttpService } from './src/app/services/http.service';
import { appInitializer } from 'src/helpers/app.initializer';
import { HttpinterceptorService } from './src/app/services/httpinterceptor.service';
import { AlertComponent } from './src/app/components/alert/alert.component';
import { SpinnerComponent } from './src/app/components/spinner/spinner.component';
import { SignupComponent } from './src/app/components/signup/signup.component';
import { ErrorinterceptorService } from './src/app/services/errorinterceptor.service';


const userModule = () => import('./src/app/modules/user/user.module').then(x => x.UserModule);


const appRoutes: Routes = [	
  {path:'', redirectTo:'home', pathMatch: 'full'},
  {path: 'login', component: LoginComponent},
  {path: 'signup', component: SignupComponent},
  {path: 'home', component: HomeComponent,canActivate:[AuthGuard] },
  {path: 'admin', component: AdminComponent,canActivate:[AuthGuard], data: {role:'Admin'}},
  {
    path: 'account' ,canActivate:[AuthGuard], data :{ role:'User'}
    ,loadChildren: userModule
  },
  {path: "**", redirectTo:'login'}
]

@NgModule({
  declarations: [
    AppComponent,
    AccountComponent,
    HomeComponent,
    AdminComponent,
    LoginComponent,
    AlertComponent,
    SpinnerComponent,
    SignupComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: appInitializer, multi: true, deps: [HttpService] },
    { provide: HTTP_INTERCEPTORS, useClass: HttpinterceptorService, multi: true },
    //{ provide: HTTP_INTERCEPTORS, useClass: ErrorinterceptorService, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
