using Recipe.Dal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Recipe.Xunit
{
    public class EF2Tests
    {
        private readonly ITestOutputHelper output;

        public EF2Tests(ITestOutputHelper output)
        {
            this.output = output;
            var converter = new XUnitConsoleLogConverter(output);
            Console.SetOut(converter);
        }

        [Fact]
        public void Ef2_Warmup()
        {
            using var dc = RecipeContext.RecipeContextFactory();
            var rec = dc.Recipes.First();
        }

        [Fact]
        public void Ef2_TagWith()
        {
            using var dc = new RecipeContext();
            var brownies = from recipe in dc.Recipes.TagWith("Testing Tag With")
                           where EF.Functions.Like(recipe.Title, "%brownie%")
                           select recipe.Title;
            foreach (var recipe in brownies)
            {
                output.WriteLine(recipe);
            }

        }
        [Fact]
        public void Ef2_Functions_Like()
        {
            using var dc = new RecipeContext();
            var brownie = "%brownie%";

            var brownies = from recipe in dc.Recipes
                           where EF.Functions.Like(recipe.Title, brownie)
                           select recipe.Title;
            foreach (var recipe in brownies)
            {
                output.WriteLine(recipe);
            }
        }

        [Fact]
        public void Ef2_View()
        {
            using var dc = new RecipeContext();
            var search = "%brownie%";
            var brownies = dc.Set<Recipe.Dal.Models.Recipe>()
               .FromSqlInterpolated($"Select * from Recipe where Title like {search}");
            Assert.True(brownies.Any());
        }
        [Fact]
        public void EfVarcharTest()
        {
            using var dc = new RecipeContext();
            var query = dc.Recipes.Where(r => r.Title.Contains("brownie"));
            var brownie = query.FirstOrDefault();
            Assert.NotNull(brownie);
        }
    }
}
