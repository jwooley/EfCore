using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Recipe.Dal.Models;

public class Recipe
{
    public long Id { get; set; }

    [MaxLength(1024)]
    [Unicode(false)]
    public string Title { get; set; }

    [Column(TypeName = "decimal")]
    public decimal? ServingQuantity { get; set; }

    [MaxLength(50)]
    public string ServingMeasure { get; set; }

    public virtual ICollection<Direction> Directions { get; set; } = new HashSet<Direction>();
    public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();

    public string[]? Categories { get; set; } = Array.Empty<string>();
}
