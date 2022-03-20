import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountSubject } from 'src/app/src/app/models/accountsubject';
import { HttpService } from 'src/app/src/app/services/http.service';
import { SpinnerService } from 'src/app/src/app/services/spinner.service';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {

  recipe:Recipe = <Recipe>{};
  imgURL:any = 'assets/no-image.png';
  file:any;
  insertorupdate:string = "insert";

  constructor(private http:HttpService, private spinner:SpinnerService, private router:Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => { 
      console.log(params['id'])
      if(params['id'] != undefined){
        
        this.insertorupdate = "update";       

        this.spinner.Spinner(true);
        this.http.getRecipeForUpdate(params['id']).subscribe(x => {
          this.recipe = x;
          this.http.getSingleImageForRecipe(this.http.accountValue.AccountId, this.recipe.RecipeId).subscribe(d => 
            {this.createImageFromBlob(d); this.spinner.Spinner(false)},
            err => { this.imgURL = 'assets/no-image.png'; this.spinner.Spinner(false) }
            )}
          , err => {console.log(err); this.spinner.Spinner(false)});

      } 
    })
  }

  createImageFromBlob(image: Blob) {
    let reader = new FileReader();
    reader.addEventListener("load", () => {
       this.imgURL = reader.result;
    }, false);
 
    if (image) {
       reader.readAsDataURL(image);
    }
    
 }

  uploadFile(files:any){
    console.log(files);
    if (files.length !== 0) {
    }
    var reader = new FileReader();

    reader.readAsDataURL(files[0]); 
    reader.onload = (_event) => { 
      this.imgURL = reader.result; 
    }
    this.file = files[0];
  }

  onSubmit(){
    const formData = new FormData();

    if (this.file == undefined) {
      let blob = new Blob(['assets/no-image.png'], {type : 'image/svg+xml'});
      formData.append('file', blob, 'no-image.png');
    }
    else{
     let fileToUpload = <File>this.file;
      formData.append('file', fileToUpload, fileToUpload.name);
    }
    
    formData.append('AccountId', this.http.accountValue.AccountId.toString());
    if(this.insertorupdate == "update"){  // provejriti jeli ovo update ili insert
    formData.append('RecipeId', this.recipe.RecipeId.toString());
    }
    formData.append('Name', this.recipe.Name.toString());
    formData.append('Description', this.recipe.Description.toString());
    formData.append('ListOfIngredients', this.recipe.ListOfIngredients.toString());

    // upload
    this.spinner.Spinner(true);
    this.http.uploadImage(formData)
      .subscribe(event => {

        this.spinner.Spinner(false);
        this.router.navigate(['../account']);
      }, err => {console.log(err);  
        this.spinner.Spinner(false);
        if(err.message == "Could not save image"){
          this.router.navigate(['../account']);
        }
      });

  }
}
