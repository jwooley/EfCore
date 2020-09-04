using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipe.Dal.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

namespace Recipe.Xunit
{
    [TestClass]
    public class RecipeTests
    {
        private readonly RecipeContext dc = RecipeContext.RecipeContextFactory();

        [TestMethod]
        public void Recipe_Load()
        {
            var items = dc.Recipes.Take(50);
            Assert.IsTrue(items.Any());
            foreach (var recipe in items)
            {
                Trace.WriteLine(recipe.Title);
                foreach (var ingredient in recipe.Ingredients)
                {
                    Trace.WriteLine($"{ingredient.Units} {ingredient.UnitType} - {ingredient.Description}");
                }
                Trace.WriteLine("-----");
            }
        }

        [TestMethod]
        public void Category_Create()
        {
            var newCategory = new Category { Description = $"Test {DateTime.Now.Ticks}" };
            dc.Categories.Add(newCategory);
            dc.SaveChanges();
            Assert.IsTrue(newCategory.Id > 0);

            // Cleanup
            dc.Categories.Remove(newCategory);
            dc.SaveChanges();
        }

        [TestMethod]
        public async Task Recipe_CanCreateBatch()
        {
            var recipe = new Recipe.Dal.Models.Recipe
            {
                Title = "Add Test",
                ServingMeasure = "Bites",
                ServingQuantity = 42
            };
            dc.Recipes.Add(recipe);
            for (int i = 0; i < 10; i++)
            {
                recipe.Ingredients.Add(new Ingredient { Description = $"Ing {i}", SortOrder = i, Units = i.ToString(), UnitType = "pn" });
                recipe.Directions.Add(new Direction { Description = $"Step {i} - Stir", LineNumber = i });
            }
            await dc.SaveChangesAsync();
            Assert.IsTrue(recipe.Id > 0);

            // Cleanup
            dc.Ingredients.RemoveRange(recipe.Ingredients);
            dc.Directions.RemoveRange(recipe.Directions);
            dc.Recipes.Remove(recipe);
            await dc.SaveChangesAsync();
        }
        [TestMethod]
        public async Task StoredProcs()
        {
            var salmon = await dc.SearchRecipeAsync("salmon");
            Assert.AreNotEqual(0, salmon.Count());
        }
        [TestMethod]
        public void StoredProcs_CanExtend()
        {
            // Note, this version lies because stored procs aren't extendable
            // so the additional query portions are done client side.
            var salmon = dc.SearchRecipeOrderedAsync("salmon");
            Assert.IsTrue(salmon.Any());
        }

        [TestMethod]
        public void Recipe_BadCodePerformsPoorly()
        {
            var Appetizers = from cat in dc.Categories
                             where cat.Description == "Appetizers"
                             select cat;
            foreach (var category in Appetizers)
            {
                foreach (var recipe in category.RecipeCategories.Select(rc => rc.Recipe).Take(50))
                {
                    Trace.WriteLine(recipe.Title);
                    if (recipe.RecipeCategories.Count > 0)
                    {
                        Trace.Write($"    Category: " + recipe.RecipeCategories.First().Category.Description);
                    }
                    if (recipe.Ingredients.Count > 0)
                    {
                        foreach (var ingredient in recipe.Ingredients.OrderBy(i => i.SortOrder))
                        {
                            Trace.Write(dc.Ingredients.SingleOrDefault(i => i.Id == ingredient.Id).Units);
                            Trace.Write($" {dc.Ingredients.SingleOrDefault(i => i.Id == ingredient.Id).UnitType} ");
                            Trace.WriteLine(dc.Ingredients.SingleOrDefault(i => i.Id == ingredient.Id).Description);
                        }
                    }
                    foreach (var directionLine in recipe.Directions.OrderBy(d => d.LineNumber))
                    {
                        Trace.WriteLine(directionLine.Description);
                    }
                }
            }
        }


        [TestMethod]
        public void Recipe_EagerLoading()
        {
            var salmon = from r in dc.Recipes
                                .Include(rec => rec.RecipeCategories).ThenInclude(rc => rc.Category)
                                .Include(rec => rec.Ingredients)
                                .Include(rec => rec.Directions)
                           where r.Title.Contains("salmon")
                           select r;

            foreach (var recipe in salmon.Take(50).ToList())
            {
                Trace.WriteLine(recipe.Title);
                Trace.WriteLine($"    Category: " + recipe.RecipeCategories.FirstOrDefault()?.Category?.Description);

                foreach (var ingredient in recipe.Ingredients.OrderBy(i => i.SortOrder))
                {
                    Trace.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions.OrderBy(d => d.LineNumber))
                {
                    Trace.WriteLine(directionLine.Description);
                }
            }
        }

        [TestMethod]
        public void Recipe_Projections()
        {
            var salmon = from r in dc.Recipes
                           where r.Title.Contains("salmon")
                           select new
                           {
                               r.Title,
                               Categories = r.RecipeCategories.Select(rc => rc.Category.Description).ToList(),
                               Ingredients = r.Ingredients.OrderBy(i => i.SortOrder).ToList(),
                               Directions = r.Directions.OrderBy(d => d.LineNumber).Select(d => d.Description).ToList()
                           };

            foreach (var recipe in salmon.Take(50).ToList())
            {
                Trace.WriteLine(recipe.Title);
                foreach (var category in recipe.Categories)
                {
                    Trace.WriteLine($"    Category: " + category);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    Trace.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    Trace.WriteLine(directionLine);
                }
            }
        }

        /// <summary>
        /// Using tolist for child collections reduces the queries issued and uses
        /// similar behavior to include except it doesn't do select *
        /// </summary>
        [TestMethod]
        public void Recipe_Projections21()
        {
            var salmon = from r in dc.Recipes
                         where r.Title.Contains("salmon")
                         select new
                         {
                             r.Title,
                             Categories = r.RecipeCategories.Select(rc => rc.Category.Description).ToList(),
                             Ingredients = r.Ingredients.OrderBy(i => i.SortOrder).ToList(),
                             Directions = r.Directions.OrderBy(d => d.LineNumber).Select(d => d.Description).ToList()
                         };

            foreach (var recipe in salmon.Take(50).ToList())
            {
                Trace.WriteLine(recipe.Title);
                foreach (var category in recipe.Categories)
                {
                    Trace.WriteLine($"    Category: " + category);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    Trace.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    Trace.WriteLine(directionLine);
                }
            }
        }

        [TestMethod]
        public void Recipe_LazyLoad()
        {
            var salmon = dc.Recipes
                .Where(r => r.Title.Contains("salmon"))
                .Take(50);

            foreach (var recipe in salmon.ToList())
            {
                Trace.WriteLine(recipe.Title);
                foreach (var category in recipe.RecipeCategories)
                {
                    Trace.WriteLine($"    Category: " + category.Category.Description);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    Trace.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    Trace.WriteLine(directionLine);
                }
            }
        }

        [TestMethod]
        public void Recipe_WithoutNavigationProperties()
        {
            var salmon = from r in dc.Recipes
                           where r.Title.Contains("salmon")
                           select new
                           {
                               r.Title,
                               Categories = dc.RecipeCategory.Where(rc => rc.RecipeId == r.Id).Select(rc => rc.Category.Description).ToList(),
                               Ingredients = dc.Ingredients.Where(i => i.RecipeId == r.Id).OrderBy(i => i.SortOrder).ToList(),
                               Directions = dc.Directions.Where(d => d.RecipeId == r.Id).OrderBy(d => d.LineNumber).Select(d => d.Description).ToList()
                           };

            foreach (var recipe in salmon.ToList())
            {
                Trace.WriteLine(recipe.Title);
                foreach (var category in recipe.Categories)
                {
                    Trace.WriteLine($"    Category: " + category);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    Trace.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    Trace.WriteLine(directionLine);
                }
            }
        }
        [TestMethod]
        public void FirstSingle()
        {
            var recipeId = dc.Recipes.First().Id;

            var recipe1 = dc.Recipes.First(r => r.Id == recipeId);
            var recipe2 = dc.Recipes.FirstOrDefault(r => r.Id == recipeId);
            var recipe3 = dc.Recipes.Single(r => r.Id == recipeId);
            var recipe4 = dc.Recipes.SingleOrDefault(r => r.Id == recipeId);

            var recipe5 = dc.Recipes.Where(r => r.Id == recipeId).First();

            Trace.WriteLine("Fetch from cache");
            var recipeCached = dc.Recipes.Find(recipeId);
            Assert.IsNotNull(recipeCached);
            Trace.WriteLine("Fetch from Local");
            var cachedAgain = dc.Recipes.Local.FirstOrDefault(r => r.Id == recipeId);
            Assert.IsNotNull(cachedAgain);
        }
    }
}
