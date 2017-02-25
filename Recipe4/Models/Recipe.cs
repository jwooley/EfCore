using System;
using System.Collections.Generic;

namespace Recipe.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            Direction = new HashSet<Direction>();
            Ingredient = new HashSet<Ingredient>();
            RecipeCategory = new HashSet<RecipeCategory>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public decimal? ServingQuantity { get; set; }
        public string ServingMeasure { get; set; }

        public virtual ICollection<Direction> Direction { get; set; }
        public virtual ICollection<Ingredient> Ingredient { get; set; }
        public virtual ICollection<RecipeCategory> RecipeCategory { get; set; }
    }
}
