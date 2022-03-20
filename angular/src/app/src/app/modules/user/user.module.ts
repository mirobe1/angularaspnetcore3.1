import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipesComponent } from './components/recipes/recipes.component';
import { RecipeComponent } from './components/recipe/recipe.component';
import { RouterModule, Routes } from '@angular/router';
import { UploadComponent } from './components/upload/upload.component';
import { FormsModule } from '@angular/forms';
import { RecipePreviewComponent } from './components/recipe-preview/recipe-preview.component';
import { IngredientsPipe } from '../../pipes/ingredients.pipe';

const routes: Routes = [
  { path: '', component: RecipesComponent },
  { path: 'recipe', component: RecipeComponent },
  { path: 'recipe/:id', component: RecipeComponent }
];

@NgModule({
  declarations: [
    RecipesComponent,
    RecipeComponent,
    UploadComponent,
    RecipePreviewComponent,
    IngredientsPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes)
  ],
  exports:[
    RouterModule
  ]

})
export class UserModule { }
