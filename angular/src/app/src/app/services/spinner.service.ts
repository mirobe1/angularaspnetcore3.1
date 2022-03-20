import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SpinnerService {
  isActive:Boolean = false;
  private subject = new Subject<Boolean>();

  getSpinnerStatus():Observable<Boolean>{
    return this.subject.asObservable();
  }

  Spinner(status:Boolean){
    this.isActive = status;
    this.subject.next(this.isActive);
  }

  constructor() { }
}
