using System;
using System.Collections.Generic;

namespace Recipe.Models
{
    public partial class Category
    {
        public Category()
        {
            RecipeCategory = new HashSet<RecipeCategory>();
        }

        public long Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RecipeCategory> RecipeCategory { get; set; }
    }
}
