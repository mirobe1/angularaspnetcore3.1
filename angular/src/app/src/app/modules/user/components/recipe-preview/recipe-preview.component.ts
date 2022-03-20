import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { HttpService } from 'src/app/src/app/services/http.service';
import { RecipeReturn } from '../../models/recipereturn';

@Component({
  selector: 'app-recipe-preview',
  templateUrl: './recipe-preview.component.html',
  styleUrls: ['./recipe-preview.component.css']
})
export class RecipePreviewComponent implements OnInit {

  @Input() recipe : RecipeReturn = <RecipeReturn>{};

  imageToShow: any;
  @Output() onDeleteRecipe : EventEmitter<number> = new EventEmitter();
  constructor(private http:HttpService) { }

  ngOnInit(): void {

    this.http.getSingleImageForRecipe(this.http.accountValue.AccountId, this.recipe.RecipeId).subscribe(d => 
      {this.createImageFromBlob(d);},
      err => { this.imageToShow = 'assets/no-image.png'; }
      )

  }

  createImageFromBlob(image: Blob) {
    let reader = new FileReader();
    reader.addEventListener("load", () => {
       this.imageToShow = reader.result;
    }, false);
 
    if (image) {
       reader.readAsDataURL(image);
    }
    
 }

 onDelete(recipeId:number){
  this.onDeleteRecipe.emit(recipeId);
 }
 

}
