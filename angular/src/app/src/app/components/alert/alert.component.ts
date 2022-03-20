import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Subscription } from 'rxjs';
import { Alert, AlertType } from '../../models/alert';
import { AlertService } from '../../services/alert.service';

@Component({ selector: 'alert', templateUrl: 'alert.component.html' })
export class AlertComponent implements OnInit, OnDestroy {

    alert:Alert= <Alert>{};
    isActive:boolean = false;
    alertSubscription: Subscription;

    constructor(private router: Router, private alertService: AlertService) {
      this.alertSubscription = this.alertService.onAlert().subscribe( x => {
        this.alert = x;
        this.isActive = true;
        if(x.autoClose){
          setTimeout(() => this.removeAlert(), 3000);
        }
      });
     }

    ngOnInit() {
    }

    ngOnDestroy() {
        this.alertSubscription.unsubscribe();
    }

    removeAlert() {
        this.alert = <Alert>{};
        this.isActive = false;
    }

    cssClasses(alert: Alert) {
        if (!alert) return;

        const classes = ['alert', 'alert-dismissable'];
                
        const alertTypeClass = {
            [AlertType.Success]: 'alert alert-success',
            [AlertType.Error]: 'alert alert-danger',
            [AlertType.Info]: 'alert alert-info',
            [AlertType.Warning]: 'alert alert-warning'
        }

        classes.push(alertTypeClass[alert.type]);

        return classes.join(' ');
    }
}