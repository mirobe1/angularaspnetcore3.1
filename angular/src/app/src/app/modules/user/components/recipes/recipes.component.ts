import { Component, OnInit } from '@angular/core';
import { HttpService } from 'src/app/src/app/services/http.service';
import { SpinnerService } from 'src/app/src/app/services/spinner.service';
import { environment } from 'src/environments/environment';
import { Recepies } from '../../models/recepies';
import { RecipeReturn } from '../../models/recipereturn';

@Component({
  selector: 'app-recipes',
  templateUrl: './recipes.component.html',
  styleUrls: ['./recipes.component.css']
})



export class RecipesComponent implements OnInit {

   pageSize = environment.recipePageSize;
   isNotFirstPage:boolean = false;
   isNotLastPage:boolean = true;
   hasData:boolean = true;
   recipes:Recepies = <Recepies>{};
   
   recipeName:string="";
   ingredientName:string ="";

  constructor(private http:HttpService, private spinner:SpinnerService) { }

  ngOnInit(): void {
    this.Filter(this.recipeName, this.ingredientName,1,this.pageSize)
  }


onFilter(){
  this.Filter(this.recipeName, this.ingredientName,1,this.pageSize)
}

Filter(recipeName:string,ingredientName:string,currentPage:number,pageSize:number){

  this.spinner.Spinner(true);

  this.http.getRecipesForAccount(this.http.accountValue.AccountId, recipeName, ingredientName, currentPage, pageSize).subscribe(x => {
    this.recipes = x;
    this.Pagination(this.recipes.CurrentPage, this.recipes.NumberOfPages);
    this.spinner.Spinner(false);
    
    }, err => {
      console.log(err);
      this.spinner.Spinner(false);
     });
}

Pagination(currentPage:number,numberOfPages:number){
  if(numberOfPages == 0){
    this.isNotFirstPage = false;
      this.isNotLastPage = false;
      this.hasData = false;
  }else{
    this.hasData = true;
    if(numberOfPages == 1){
      this.isNotFirstPage = false;
      this.isNotLastPage = false;
    }else if(numberOfPages == currentPage){
      this.isNotFirstPage = true;
      this.isNotLastPage = false;
    }else if(numberOfPages > 1  && currentPage == 1){
      this.isNotFirstPage = false;
      this.isNotLastPage = true;
    }else{
      this.isNotFirstPage = true;
      this.isNotLastPage = true;
    }
  }
 
}

Paging(whichPage:string){
  var page = 1;
   if(whichPage == 'Prev'){
    page = (this.recipes.CurrentPage - 1);
  }else if(whichPage == 'Next'){
    page = (this.recipes.CurrentPage + 1);
  }else if(whichPage == 'Last'){
    page = this.recipes.NumberOfPages;
  }
  this.Filter(this.recipeName,this.ingredientName,page,this.pageSize);
}


deleteRecipe(recipeId:number){
  console.log(recipeId);
  this.spinner.Spinner(true);
  this.http.deleteRecipe(this.http.accountValue.AccountId, recipeId).subscribe(x => 
    {
      this.onFilter();
      this.spinner.Spinner(false);
    }
    ,err => 
    {
      console.log(err);
      this.spinner.Spinner(false);
    }
    )
}

}
