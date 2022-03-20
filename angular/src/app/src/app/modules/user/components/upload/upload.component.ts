import { HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { HttpService } from 'src/app/src/app/services/http.service';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
  
  public progress: number = 0;
  public message: string = '';
  recipe:Recipe = <Recipe>{};

  imgURL:any = null;

  @Output() public onUploadFinished = new EventEmitter();
  constructor(private http:HttpService) { }

  ngOnInit(): void {
  }

  public uploadFile = (files:any) => {
    if (files.length === 0) {
      return;
    }
    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    formData.append('id','1');

    var reader = new FileReader();
    //this.imagePath = files;
    reader.readAsDataURL(files[0]); 
    reader.onload = (_event) => { 
      this.imgURL = reader.result; 
    }

    // this.http.uploadImage(formData)
    //   .subscribe(event => {
    //     console.log(event);
    //     if (event.type === HttpEventType.UploadProgress)
    //       this.progress = Math.round(100 * event.loaded / event.total);
    //     else if (event.type === HttpEventType.Response) {
    //       this.message = 'Upload success.';
    //       this.onUploadFinished.emit(event.body);
    //     }
    //   });
  }

}
