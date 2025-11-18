using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Recipe.Dal.Models
{
    //[ValidatableType]
    [Table("Recipe")]
    public partial class Recipe
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(1024)]
        [Unicode(false)]
        public string Title { get; set; }

        [Column(TypeName = "decimal")]
        [Range(0,1000)]
        public decimal? ServingQuantity { get; set; }

        [MaxLength(50)]
        public string ServingMeasure { get; set; }

        public virtual ICollection<Direction> Directions { get; set; } = new HashSet<Direction>();
        public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}
