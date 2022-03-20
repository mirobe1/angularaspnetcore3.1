using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace coreproject.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        //public DbSet<Ingredient> Ingredients { get; set; }

        public string DbPath { get; }

        public ApplicationContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "coreporject.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public class Account
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
        public string Role { get; set; }

        public List<Recipe> Recipes { get; } = new List<Recipe>();
    }

    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public List<Ingredient> Ingredients { get; } = new List<Ingredient>();
    }

    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public int RecipeId { get; set; }
        //public Recipe Recipe { get; set; }
    }
}