using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using coreproject.Model;
using Microsoft.EntityFrameworkCore;

namespace coreproject.Services
{
    public interface IRecipeService{

        public RecipeRequest GetRecipeForUpdate(int recipeId);
        public int InsertRecipe(RecipeRequest rr);
        public int UpdateRecipe(int recipeId,RecipeRequest rr);
        public string UpdateImageNameAndPath(int recipeId, string imageName, string imagePath);
        public RecipesReturn GetRecipes(int accountId, string recipeName, string ingredientName, int currentPage, int pageSize);
        public void DeleteRecipe(int recipeId);

    }
    public class RecipeService : IRecipeService
    {
        public RecipeRequest GetRecipeForUpdate(int recipeId)
        {
            try{
                using(var db = new ApplicationContext())
                {
                    var recipe = db.Recipes.Include(d => d.Ingredients).Where(d => d.RecipeId == recipeId).FirstOrDefault();
                    RecipeRequest rec = new RecipeRequest();
                    rec.RecipeId = recipeId;
                    rec.Name = recipe.Name;
                    rec.Description = recipe.Description;
                    rec.ListOfIngredients = String.Join(",",recipe.Ingredients.Select(m => m.Name).ToList());

                    return rec;
                }
            }
            catch
            {
                throw new Exception("Could not get Recipe");
            }
        }
        public int InsertRecipe(RecipeRequest rr)
        {
            try{
                using(var db = new ApplicationContext())
                {
                    Recipe recipe = new Recipe();

                    recipe.AccountId = rr.AccountId;
                    recipe.Name = rr.Name;
                    recipe.Description = rr.Description;
                    List<string> ingredients = new List<string>(rr.ListOfIngredients.Split(','));
                    
                    foreach(string ing in ingredients)
                    {
                        if(ing != string.Empty)
                        {
                            Ingredient ingredient = new Ingredient();
                            ingredient.Name = ing;
                            recipe.Ingredients.Add(ingredient);
                        }
                    }

                    db.Recipes.Add(recipe);
                    db.SaveChanges();
                    
                    return recipe.RecipeId;

                }
            }
            catch
            {
                throw new Exception("Could not create Recipe");
            }
        }

        public int UpdateRecipe(int recipeId, RecipeRequest rr)
        {
            try{
                using(var db = new ApplicationContext())
                {
                    Recipe recipe = db.Recipes.Include(d => d.Ingredients).Where(d => d.RecipeId == recipeId).FirstOrDefault();
                    recipe.Name = rr.Name;
                    recipe.Description = rr.Description;
                    
                    var ingredientsFromDatabase = recipe.Ingredients.Select(d => d.Name).ToList();
                    var ingredientsFromApp = rr.ListOfIngredients.Split(",").ToList();

                    List<string> ingredientsToAdd = ingredientsFromApp.Except(ingredientsFromDatabase).ToList();
                    List<string> ingredentsToDelete = ingredientsFromDatabase.Except(ingredientsFromApp).ToList();

                    foreach(string ing in ingredientsToAdd)
                    {
                        if(ing != string.Empty)
                        {
                            Ingredient ingredient = new Ingredient();
                            ingredient.Name = ing;
                            ingredient.IngredientId = rr.RecipeId;
                            recipe.Ingredients.Add(ingredient);
                        }
                    }

                    foreach(string ing in ingredentsToDelete)
                    {
                        recipe.Ingredients.Remove(recipe.Ingredients.Where(d => d.Name == ing).FirstOrDefault());
                    }

                    db.SaveChanges();

                    
                    return recipe.RecipeId;

                }
            }
            catch
            {
                throw new Exception("Could not update Recipe");
            }
        }

        public string UpdateImageNameAndPath(int recipeId, string imageName, string imagePath)
        {
             try{
                    using(var db = new ApplicationContext())
                    {

                        var recipe = db.Recipes.Where(d => d.RecipeId == recipeId).FirstOrDefault();
                        recipe.ImageName = imageName;
                        recipe.ImagePath = imagePath;
                        db.SaveChanges();

                        return imageName;
                    }
                }
                catch
                {
                    throw new Exception("Could not update image in database for Recipe");
                }
        }

        public RecipesReturn GetRecipes(int accountId, string recipeName, string ingredientName, int currentPage, int pageSize)
        {
            try
            {
                if(recipeName == "No")
                {
                    recipeName = "";
                }
                if(ingredientName == "No")
                {
                   ingredientName = "";
                }

                using(var db = new ApplicationContext())
                {
                    var numberOfRecords = db.Recipes.Include(d => d.Ingredients).Where(d => d.AccountId == accountId && d.Ingredients.Any(x => x.Name.Contains(ingredientName)) && d.Name.Contains(recipeName)).Count();
                    
                    int numberOfPages = 0;
                    if(numberOfRecords % pageSize == 0){
                        numberOfPages = numberOfRecords / pageSize;
                    }else{
                        numberOfPages = (Convert.ToInt32(numberOfRecords / pageSize)) + 1;
                    }

                    var recipes = db.Recipes.Include(d => d.Ingredients).Where(d => d.AccountId == accountId && d.Ingredients.Any(x => x.Name.Contains(ingredientName)) && d.Name.Contains(recipeName)).Skip(currentPage > 0 ? ( ( currentPage - 1 ) * pageSize ) : 0).Take(pageSize).ToList();
                    
                    var result = new RecipesReturn();
                    result.Recipes = recipes;
                    result.CurrentPage = currentPage;
                    result.NumberOfPages = numberOfPages;

                    return result;
                }
            }
            catch
            {
                throw new Exception("Could not get recipes");
            }
        }


        public void DeleteRecipe(int recipeId)
        {
            try
            {
                using(var db = new ApplicationContext())
                {
                    Recipe recipe = db.Recipes.Where(d => d.RecipeId == recipeId).FirstOrDefault();
                    db.Recipes.Remove(recipe);                   
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new Exception("Could not delete recipe");
            }
        }

    }
}