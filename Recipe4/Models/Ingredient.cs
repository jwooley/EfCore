using System;
using System.Collections.Generic;

namespace Recipe.Models
{
    public partial class Ingredient
    {
        public long Id { get; set; }
        public int? SortOrder { get; set; }
        public string Units { get; set; }
        public string UnitType { get; set; }
        public string Description { get; set; }
        public long? RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
