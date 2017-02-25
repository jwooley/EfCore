using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Recipe.Models
{
    public partial class RecipeContext : DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeCategory> RecipeCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"data source=.;initial catalog=recipecore;integrated security=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "dbo");

                entity.Property(e => e.Description).HasMaxLength(50);
            });

            modelBuilder.Entity<Direction>(entity =>
            {
                entity.ToTable("Direction", "dbo");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("IX_Recipe_RecipeId");

                entity.HasIndex(e => new { e.RecipeId, e.LineNumber })
                    .HasName("IX_Directions_RecipeLineNumber");

                entity.HasIndex(e => new { e.LineNumber, e.Description, e.RecipeId, e.Id })
                    .HasName("_dta_index_Directions_5_709577566__K4_K1_2_3");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Direction)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_dbo.Directions_dbo.Recipes_Recipe_RecipeId");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "dbo");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("IX_Recipe_RecipeId");

                entity.HasIndex(e => new { e.SortOrder, e.Units, e.UnitType, e.Description, e.RecipeId, e.Id })
                    .HasName("_dta_index_Ingredients_5_661577395__K6_K1_2_3_4_5");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.UnitType).HasMaxLength(50);

                entity.Property(e => e.Units).HasMaxLength(50);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Ingredient)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_dbo.Ingredients_dbo.Recipes_Recipe_RecipeId");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe", "dbo");

                entity.HasIndex(e => e.Title)
                    .HasName("IX_Recipes_Title");

                entity.Property(e => e.ServingMeasure).HasMaxLength(50);

                entity.Property(e => e.ServingQuantity).HasColumnType("decimal");

                entity.Property(e => e.Title).HasMaxLength(1024);
            });

            modelBuilder.Entity<RecipeCategory>(entity =>
            {
                entity.HasKey(e => new { e.RecipeId, e.CategoryId })
                    .HasName("PK_dbo.RecipeCategories");

                entity.ToTable("RecipeCategory", "dbo");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("IX_Category_CategoryId");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("IX_Recipe_RecipeId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.RecipeCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_dbo.RecipeCategories_dbo.Categories_Category_CategoryId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeCategory)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_dbo.RecipeCategories_dbo.Recipes_Recipe_RecipeId");
            });
        }
    }
}