using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Recipe.Dal.Models
{
    public partial class RecipeContext : DbContext
    {
        //public static readonly LoggerFactory ConsoleLoggerFactory 
        //    = new LoggerFactory(new[] 
        //    {
        //        new ConsoleLoggerProvider((category, level) => 
        //            category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information,
        //            true)
        //    });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Information)
            );
            var loggerFactory = serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();

            optionsBuilder.UseSqlServer(@"data source=.;initial catalog=recipecore;integrated security=true;multipleactiveresultsets=True;",
                options =>
                {
                    options.EnableRetryOnFailure(maxRetryCount: 3);
                    options.MaxBatchSize(10);
                })
                .UseLoggerFactory(loggerFactory)
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Direction>(entity =>
            {
                entity.HasIndex(e => e.RecipeId)
                    .HasName("IX_Recipe_RecipeId");

                entity.HasIndex(e => new { e.RecipeId, e.LineNumber })
                    .HasName("IX_Directions_RecipeLineNumber");

                entity.HasIndex(e => new { e.LineNumber, e.Description, e.RecipeId, e.Id })
                    .HasName("_dta_index_Directions_5_709577566__K4_K1_2_3");
            });
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasIndex(e => e.RecipeId)
                    .HasName("IX_Recipe_RecipeId");

                entity.HasIndex(e => new { e.SortOrder, e.Units, e.UnitType, e.Description, e.RecipeId, e.Id })
                    .HasName("_dta_index_Ingredients_5_661577395__K6_K1_2_3_4_5");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasIndex(e => e.Title)
                    .HasName("IX_Recipes_Title");

                //entity.Property(e => e.Id).ValueGeneratedNever();
            });
            //modelBuilder.Entity<Recipe>().HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<RecipeCategory>(entity =>
            {
                entity.HasKey(e => new { e.RecipeId, e.CategoryId })
                    .HasName("PK_dbo.RecipeCategories");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("IX_Category_CategoryId");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("IX_Recipe_RecipeId");
            });
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeCategory> RecipeCategory { get; set; }
    }
}