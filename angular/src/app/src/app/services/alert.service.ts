import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Alert, AlertType } from '../models/alert';

@Injectable({ providedIn: 'root' })
export class AlertService {
    private subject = new Subject<Alert>();

    onAlert(): Observable<Alert> {
        return this.subject.asObservable();
    }

    // convenience methods
    success(message: string, options?: any) {
        this.alert(<Alert>{ ...options, type: AlertType.Success, message });
    }

    error(message: string, options?: any) {
        this.alert(<Alert>{ ...options, type: AlertType.Error, message });
    }

    info(message: string, options?: any) {
        this.alert(<Alert>{ ...options, type: AlertType.Info, message });
    }

    warn(message: string, options?: any) {
        this.alert(<Alert>{ ...options, type: AlertType.Warning, message });
    }

    // core alert method
    alert(alert: Alert) {
        alert.autoClose = (alert.autoClose === undefined ? true : alert.autoClose);
        this.subject.next(alert);
    }

    clear() {
      this.subject.next(<Alert>{ });
  }
}