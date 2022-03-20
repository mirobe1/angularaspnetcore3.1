import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.css']
})
export class SpinnerComponent implements OnInit,OnDestroy {
  
  isSpinnerActive: Boolean = false;
  subscription : Subscription;

  constructor(private spinner:SpinnerService) {
    this.subscription = this.spinner.getSpinnerStatus().subscribe(x => {
      this.isSpinnerActive = x
    });
   }
  ngOnInit(): void {
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
}

}
