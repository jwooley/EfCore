using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Dal.Models;


/// <summary>
/// current tooling requires launchable host to use migrations.
/// Dummy MigrationsConcole project handles this requirement.
/// To add migrations use:
/// dotnet ef --startup-project ../MigrationsConcole migrations add {name}
/// To apply migrations use:
/// dotnet ef --startup-project ../MigrationsConsole database update
/// </summary>
public partial class RecipeContext 
{

    public RecipeContext()
    {
    }

    public static RecipeContext RecipeContextFactory()
    {
        var context = new RecipeContext();
        return context;
    }

    /// <summary>
    /// Constructor used when unit testing with in memory provider
    /// </summary>
    /// <param name="options"></param>
    public RecipeContext(DbContextOptions<RecipeContext> options) : base(options) { }

    public async Task<IEnumerable<RecipeOld>> SearchRecipeAsync(string searchString)
    {
        return await Set<RecipeOld>().FromSqlInterpolated($"sRecipeSearch @searchText={searchString}").ToListAsync();
    }

    public IEnumerable<RecipeOld> SearchRecipeOrderedAsync(string searchString)
    {
        return Set<RecipeOld>()
            .FromSqlInterpolated($"sRecipeSearch @searchText={searchString}")
            .AsEnumerable()
            .OrderBy(r => r.Title);
    }

    public IEnumerable<Recipe> GetRecipesUncompiled(string searchCriteria)
    {
        return Recipes.Where(r =>
                r.Title.Contains(searchCriteria)
                || r.Ingredients.Any(i => i.Description.Contains(searchCriteria)));
    }

    public static readonly Func<RecipeContext, string, IEnumerable<Recipe>> GetRecipesCompiled = 
        EF.CompileQuery((RecipeContext db, string searchCriteria) => 
            db.Recipes.Where(r => 
                r.Title.Contains(searchCriteria)
                || r.Ingredients.Any(i => i.Description.Contains(searchCriteria))));

}
