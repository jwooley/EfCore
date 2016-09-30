
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication
{
    [Table("Recipe")]
    public class Recipe 
    {
        public long Id {get;set;}
        public string Title {get;set;}
        public decimal ServingQuantity {get;set;}
        public string ServingMeasure {get;set;}
    }
    public class RecipeContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=.;initial catalog=recipecore;integrated security=true");
        }
        public virtual  DbSet<Recipe> Recipes {get;set;}
    }
}