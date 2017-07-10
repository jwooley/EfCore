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
        public void Ef2_Functions_Like()
        {
            using (var dc = new RecipeContext())
            {
                var brownies = from recipe in dc.Recipes
                               where EF.Functions.Like(recipe.Title, "%brownie%")
                               select recipe.Title;
                foreach (var recipe in brownies)
                {
                    Console.WriteLine(recipe);
                }
            }
        }
    }
}
