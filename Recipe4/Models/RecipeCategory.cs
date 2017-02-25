using System;
using System.Collections.Generic;

namespace Recipe.Models
{
    public partial class RecipeCategory
    {
        public long RecipeId { get; set; }
        public long CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
