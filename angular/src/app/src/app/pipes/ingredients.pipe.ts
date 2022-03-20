import { Pipe, PipeTransform } from '@angular/core';
import { Ingredient } from '../modules/user/models/ingredient';

@Pipe({
  name: 'ingredients'
})
export class IngredientsPipe implements PipeTransform {

  transform(value:Ingredient[]): string {
      var ingr:string[] = value.map(x => x.Name);
      return ingr.join(',');
  }

}
