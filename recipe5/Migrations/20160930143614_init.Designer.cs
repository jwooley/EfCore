using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ConsoleApplication;

namespace recipe5.Migrations
{
    [DbContext(typeof(RecipeContext))]
    [Migration("20160930143614_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ConsoleApplication.Recipe", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ServingMeasure");

                    b.Property<decimal>("ServingQuantity");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Recipe");
                });
        }
    }
}
