using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Recipe.Dal.Models
{
    public partial class RecipeContext : DbContext
    {
        public static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
             optionsBuilder
                // See https://erikej.github.io/efcore/2020/05/18/ef-core-simple-logging.html for details on LogTo
                .LogTo(Console.WriteLine,
                    new[]
                    {
                        DbLoggerCategory.Database.Command.Name
                    },
                    LogLevel.Information,
                    DbContextLoggerOptions.SingleLine | DbContextLoggerOptions.UtcTime)
                .UseSqlServer(@"data source=.,1401;initial catalog=recipecore;User Id=sa;Password=SuperSecretP@ssw0rd;multipleactiveresultsets=True;",
                options =>
                {
                    options.EnableRetryOnFailure(maxRetryCount: 3);
                    options.MaxBatchSize(10);
                })
                //.EnableSensitiveDataLogging()
                //.UseLoggerFactory(ContextLoggerFactory)
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Direction>(entity =>
            {
                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("IX_Recipe_RecipeId");

                entity.HasIndex(e => new { e.RecipeId, e.LineNumber })
                    .HasDatabaseName("IX_Directions_RecipeLineNumber");

                entity.HasIndex(e => new { e.LineNumber, e.Description, e.RecipeId, e.Id })
                    .HasDatabaseName("_dta_index_Directions_5_709577566__K4_K1_2_3");
            });
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("IX_Recipe_RecipeId");

                entity.HasIndex(e => new { e.SortOrder, e.Units, e.UnitType, e.Description, e.RecipeId, e.Id })
                    .HasDatabaseName("_dta_index_Ingredients_5_661577395__K6_K1_2_3_4_5");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasIndex(e => e.Title)
                    .HasDatabaseName("IX_Recipes_Title");
            });
            //modelBuilder.Entity<Recipe>().HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<RecipeCategory>(entity =>
            {
                entity.HasKey(e => new { e.RecipeId, e.CategoryId })
                    .HasName("PK_dbo.RecipeCategories");

                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("IX_Category_CategoryId");

                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("IX_Recipe_RecipeId");
            });
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeCategory> RecipeCategory { get; set; }
    }
}
