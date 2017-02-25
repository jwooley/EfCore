using System;
using System.Collections.Generic;

namespace Recipe.Models
{
    public partial class Direction
    {
        public long Id { get; set; }
        public long LineNumber { get; set; }
        public string Description { get; set; }
        public long? RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
