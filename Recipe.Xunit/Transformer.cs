using Recipe.Dal.Models;

namespace Recipe.Xunit;

public class Transformer
{
    private readonly RecipeContext dc = RecipeContext.RecipeContextFactory();

    //[Fact]
    //public async Task TransformToSimpleRecipe()
    //{
    //    await dc.Database.ExecuteSqlRawAsync("Truncate table RecipeSimple");
    //    var recipies = await dc.Recipes
    //        .Select(r => new Dal.Models.Recipe{
    //            Title = r.Title,
    //            ServingQuantity = r.ServingQuantity,
    //            ServingMeasure = r.ServingMeasure,
    //            Categories = r.Categories.Select(c => c.Description).ToArray(),
    //            Id = r.Id
    //        })
    //        //.Take(10)
    //        .ToListAsync();
    //    foreach (var recipe in recipies)
    //    {
    //        dc.RecipeSimples.Add(recipe);
    //    }
    //    await dc.SaveChangesAsync();

    //    var mainDishes = dc.RecipeSimples.Where(r => r.Categories.Contains("Main dish"));
    //    //Assert.Equal(6, mainDishes.Count());
    }
//}
