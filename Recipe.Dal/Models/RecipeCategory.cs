using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipe.Dal.Models
{
    public partial class RecipeCategory
    {
        public long RecipeId { get; set; }
        public long CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
