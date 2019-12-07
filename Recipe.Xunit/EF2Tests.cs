using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipe.Dal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Recipe.Xunit
{
    [TestClass]
    public class EF2Tests
    {

        [TestMethod]
        public void Ef2_Warmup()
        {
            using var dc = RecipeContext.RecipeContextFactory();
            var rec = dc.Recipes.First();
        }

        [TestMethod]
        public void Ef2_TagWith()
        {
            using var dc = new RecipeContext();
            var brownies = from recipe in dc.Recipes.TagWith("Testing Tag With")
                           where EF.Functions.Like(recipe.Title, "%brownie%")
                           select recipe.Title;
            foreach (var recipe in brownies)
            {
                Console.WriteLine(recipe);
            }

        }
        [TestMethod]
        public void Ef2_Functions_Like()
        {
            using var dc = new RecipeContext();
            var brownie = "%brownie%";

            var brownies = from recipe in dc.Recipes
                           where EF.Functions.Like(recipe.Title, brownie)
                           select recipe.Title;
            foreach (var recipe in brownies)
            {
                Console.WriteLine(recipe);
            }
        }

        [TestMethod]
        public void Ef2_View()
        {
            using var dc = new RecipeContext();
            var search = "%brownie%";
            var brownies = dc.Set<Recipe.Dal.Models.Recipe>()
               .FromSqlInterpolated($"Select * from Recipe where Title like {search}");
            Assert.IsTrue(brownies.Any());
        }
        [TestMethod]
        public void EfVarcharTest()
        {
            using var dc = new RecipeContext();
            var query = dc.Recipes.Where(r => r.Title.Contains("brownie"));
            var brownie = query.FirstOrDefault();
            Assert.IsNotNull(brownie);
        }
    }
}
