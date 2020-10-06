using Microsoft.EntityFrameworkCore;
using Recipe.Dal.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Recipe.Xunit
{

    public class RecipeTests
    {
        private readonly ITestOutputHelper output;
        public RecipeTests(ITestOutputHelper output)
        {
            this.output = output;
            var converter = new XUnitConsoleLogConverter(output);
            Console.SetOut(converter);
        }
        private readonly RecipeContext dc = RecipeContext.RecipeContextFactory();

        [Fact]
        public void Recipe_Load()
        {
            var items = dc.Recipes.Take(50);
            Assert.True(items.Any());
            foreach (var recipe in items)
            {
                output.WriteLine(recipe.Title);
                foreach (var ingredient in recipe.Ingredients)
                {
                    output.WriteLine($"{ingredient.Units} {ingredient.UnitType} - {ingredient.Description}");
                }
                output.WriteLine("-----");
            }
        }

        [Fact]
        public void Category_Create()
        {
            var newCategory = new Category { Description = $"Test {DateTime.Now.Ticks}" };
            dc.Categories.Add(newCategory);
            dc.SaveChanges();
            Assert.True(newCategory.Id > 0);

            // Cleanup
            dc.Categories.Remove(newCategory);
            dc.SaveChanges();
        }

        [Fact]
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
            Assert.True(recipe.Id > 0);

            // Cleanup
            dc.Ingredients.RemoveRange(recipe.Ingredients);
            dc.Directions.RemoveRange(recipe.Directions);
            dc.Recipes.Remove(recipe);
            await dc.SaveChangesAsync();
        }
        [Fact]
        public async Task StoredProcs()
        {
            var salmon = await dc.SearchRecipeAsync("salmon");
            Assert.NotEmpty(salmon);
        }
        [Fact]
        public void StoredProcs_CanExtend()
        {
            // Note, this version lies because stored procs aren't extendable
            // so the additional query portions are done client side.
            var salmon = dc.SearchRecipeOrderedAsync("salmon");
            Assert.True(salmon.Any());
        }

        [Fact]
        public void Recipe_BadCodePerformsPoorly()
        {
            var Appetizers = from cat in dc.Categories
                             where cat.Description == "Appetizers"
                             select cat;
            foreach (var category in Appetizers)
            {
                foreach (var recipe in category.RecipeCategories.Select(rc => rc.Recipe).Take(50))
                {
                    output.WriteLine(recipe.Title);
                    if (recipe.RecipeCategories.Count > 0)
                    {
                        output.WriteLine($"    Category: " + recipe.RecipeCategories.First().Category.Description);
                    }
                    if (recipe.Ingredients.Count > 0)
                    {
                        foreach (var ingredient in recipe.Ingredients.OrderBy(i => i.SortOrder))
                        {
                            output.WriteLine(dc.Ingredients.SingleOrDefault(i => i.Id == ingredient.Id).Units);
                            output.WriteLine($" {dc.Ingredients.SingleOrDefault(i => i.Id == ingredient.Id).UnitType} ");
                            output.WriteLine(dc.Ingredients.SingleOrDefault(i => i.Id == ingredient.Id).Description);
                        }
                    }
                    foreach (var directionLine in recipe.Directions.OrderBy(d => d.LineNumber))
                    {
                        output.WriteLine(directionLine.Description);
                    }
                }
            }
        }


        [Fact]
        public void Recipe_EagerLoading()
        {
            var salmon = from r in dc.Recipes
                                .Include(rec => rec.RecipeCategories).ThenInclude(rc => rc.Category)
                                .Include(rec => rec.Ingredients.OrderBy(i => i.SortOrder))
                                .Include(rec => rec.Directions.OrderBy(d => d.LineNumber))
                           where r.Title.Contains("salmon")
                           select r;

            foreach (var recipe in salmon.Take(50).ToList())
            {
                output.WriteLine(recipe.Title);
                output.WriteLine($"    Category: " + recipe.RecipeCategories.FirstOrDefault()?.Category?.Description);

                foreach (var ingredient in recipe.Ingredients)
                {
                    output.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions.OrderBy(d => d.LineNumber))
                {
                    output.WriteLine(directionLine.Description);
                }
            }
        }

        [Fact]
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
                output.WriteLine(recipe.Title);
                foreach (var category in recipe.Categories)
                {
                    output.WriteLine($"    Category: " + category);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    output.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    output.WriteLine(directionLine);
                }
            }
        }

        /// <summary>
        /// Using tolist for child collections reduces the queries issued and uses
        /// similar behavior to include except it doesn't do select *
        /// </summary>
        [Fact]
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
                output.WriteLine(recipe.Title);
                foreach (var category in recipe.Categories)
                {
                    output.WriteLine($"    Category: " + category);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    output.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    output.WriteLine(directionLine);
                }
            }
        }

        [Fact]
        public void Recipe_LazyLoad()
        {
            var salmon = dc.Recipes
                .Where(r => r.Title.Contains("salmon"))
                .Take(50);

            foreach (var recipe in salmon.ToList())
            {
                output.WriteLine(recipe.Title);
                foreach (var category in recipe.RecipeCategories)
                {
                    output.WriteLine($"    Category: " + category.Category.Description);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    output.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    output.WriteLine(directionLine.Description);
                }
            }
        }

        [Fact]
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
                output.WriteLine(recipe.Title);
                foreach (var category in recipe.Categories)
                {
                    output.WriteLine($"    Category: " + category);
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    output.WriteLine($"{ingredient.Units} {ingredient.UnitType}: {ingredient.Description}");
                }

                foreach (var directionLine in recipe.Directions)
                {
                    output.WriteLine(directionLine);
                }
            }
        }
        [Fact]
        public void FirstSingle()
        {
            var recipeId = dc.Recipes.First().Id;

            var recipe1 = dc.Recipes.First(r => r.Id == recipeId);
            var recipe2 = dc.Recipes.FirstOrDefault(r => r.Id == recipeId);
            var recipe3 = dc.Recipes.Single(r => r.Id == recipeId);
            var recipe4 = dc.Recipes.SingleOrDefault(r => r.Id == recipeId);

            var recipe5 = dc.Recipes.Where(r => r.Id == recipeId).First();

            output.WriteLine("Fetch from cache");
            var recipeCached = dc.Recipes.Find(recipeId);
            Assert.NotNull(recipeCached);
            output.WriteLine("Fetch from Local");
            var cachedAgain = dc.Recipes.Local.FirstOrDefault(r => r.Id == recipeId);
            Assert.NotNull(cachedAgain);
        }
    }
}
