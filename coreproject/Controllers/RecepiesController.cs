using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using coreproject.Attributes;
using coreproject.Model;
using coreproject.Services;
using Microsoft.AspNetCore.Mvc;

namespace coreproject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecepiesController: ControllerBase
    {

         private readonly IRecipeService _recipeService;

         public RecepiesController(IRecipeService recipeService)
         {
             _recipeService = recipeService;
         }

        [Authorize("User")]
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {

                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                RecipeRequest rr = new RecipeRequest();
                
                rr.AccountId = Convert.ToInt32(formCollection.Where(k => k.Key == "AccountId").Select(d => d.Value).FirstOrDefault());
                rr.Name = formCollection.Where(k => k.Key == "Name").Select(d => d.Value).FirstOrDefault().ToString();
                rr.Description = formCollection.Where(k => k.Key == "Description").Select(d => d.Value).FirstOrDefault().ToString();
                rr.ListOfIngredients = formCollection.Where(k => k.Key == "ListOfIngredients").Select(d => d.Value).FirstOrDefault().ToString();


                int recipeId;
                if(formCollection.Where(k => k.Key == "RecipeId").Any()) // update
                {
                    recipeId = _recipeService.UpdateRecipe(Convert.ToInt32(formCollection.Where(k => k.Key == "RecipeId").Select(d => d.Value).FirstOrDefault()), rr);
                }
                else    // insert
                {
                    recipeId = _recipeService.InsertRecipe(rr);
                }

                var MainFolderName = Path.Combine("Resources", "Images");
                var MainFolderPath = Path.Combine(Directory.GetCurrentDirectory(), MainFolderName);

                // check if Account folder exists
                var AccountFolderPath = Path.Combine(MainFolderPath, rr.AccountId.ToString());
                if (!Directory.Exists(AccountFolderPath))  
                {  
                    Directory.CreateDirectory(AccountFolderPath);  
                }

                // check if Recipe foder exists

                var RecipeFolderPath = Path.Combine(AccountFolderPath, recipeId.ToString());
                if (!Directory.Exists(RecipeFolderPath))  
                {  
                    Directory.CreateDirectory(RecipeFolderPath);  
                }

                if (file.Length > 0 && file.FileName != "no-image.png")
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(RecipeFolderPath, fileName);
                    var dbPath = Path.Combine(RecipeFolderPath, fileName);

                    // if image exists this is update so we delete old file
                    System.IO.DirectoryInfo di = new DirectoryInfo(RecipeFolderPath);
                    foreach (FileInfo fileToDelete in di.GetFiles())
                    {
                        System.GC.Collect(); 
                        System.GC.WaitForPendingFinalizers(); 
                        fileToDelete.Delete(); 
                    }

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var imagePath = _recipeService.UpdateImageNameAndPath(recipeId, fileName, RecipeFolderPath);

                    return Ok(new { dbPath });
                }
                else
                {
                    throw new Exception("Could not save image");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Authorize("User")]
        [Route("recipes/{accountId}/{recipeName}/{ingredientName}/{currentPage}/{pageSize}")]
        public ActionResult GetRecipes(int accountId, string recipeName, string ingredientName, int currentPage, int pageSize)
        {

            try
            {
                var recipes = _recipeService.GetRecipes(accountId, recipeName, ingredientName,currentPage,pageSize);
                return Ok(recipes);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Authorize("User")]
        [Route("recipe-for-update/{recipeId}")]
        public ActionResult GetRecipeForUpdate(int recipeId)
        {
            try
            {
                var recipe = _recipeService.GetRecipeForUpdate(recipeId);
                return Ok(recipe);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Authorize("User")]
        [Route("recipe-image/{accountId}/{recipeId}")]
        public ActionResult GetRecipeImage(int accountId, int recipeId)
        {
            try
            {
                var MainFolderName = Path.Combine("Resources", "Images");
                var MainFolderPath = Path.Combine(Directory.GetCurrentDirectory(), MainFolderName);

                // check if Account folder exists
                var AccountFolderPath = Path.Combine(MainFolderPath, accountId.ToString());
                if (Directory.Exists(AccountFolderPath))  
                {  
                    var RecipeFolderPath = Path.Combine(AccountFolderPath, recipeId.ToString());
                    if (Directory.Exists(RecipeFolderPath))  
                    {  
                        var files = Directory.GetFiles(RecipeFolderPath);
                        var image =  System.IO.File.OpenRead(files[0]);
                        return PhysicalFile(files[0], "image/jpeg");
                    }
                }

                throw new Exception("No image");

             }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }   

        }

        [HttpDelete]
        [Authorize("User")]
        [Route("recipe-delete/{accountId}/{recipeId}")]
        public ActionResult DeleteRecipe(int accountId, int recipeId)
        {
            try
            {
                _recipeService.DeleteRecipe(recipeId);
                
                var MainFolderName = Path.Combine("Resources", "Images");
                var MainFolderPath = Path.Combine(Directory.GetCurrentDirectory(), MainFolderName);

                // check if Account folder exists
                var AccountFolderPath = Path.Combine(MainFolderPath, accountId.ToString());
                if (Directory.Exists(AccountFolderPath))  
                {
                    // check if Recipe foder exists
                    var RecipeFolderPath = Path.Combine(AccountFolderPath, recipeId.ToString());
                    if (Directory.Exists(RecipeFolderPath))  
                    {  
                        System.GC.Collect(); 
                        System.GC.WaitForPendingFinalizers(); 
                        Directory.Delete(RecipeFolderPath, true);
                    } 
                }               

                return Ok();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}