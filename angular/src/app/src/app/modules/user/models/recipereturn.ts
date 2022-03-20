import { Ingredient } from "./ingredient";

export interface RecipeReturn {
    RecipeId:number,
    Name:string,
    Description:string,
    Ingredients:Ingredient[]
}