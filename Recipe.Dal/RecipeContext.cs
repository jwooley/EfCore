using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Dal.Models
{

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

        public async Task<IEnumerable<Recipe>> SearchRecipeAsync(string searchString)
        {
            return await Set<Recipe>().FromSqlInterpolated($"sRecipeSearch @searchText={searchString}").ToListAsync();
        }

        public IEnumerable<Recipe> SearchRecipeOrderedAsync(string searchString)
        {
            return Set<Recipe>()
                .FromSqlInterpolated($"sRecipeSearch @searchText={searchString}")
                .AsEnumerable()
                .OrderBy(r => r.Title);
        }
    }
}
