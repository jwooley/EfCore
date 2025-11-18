using Recipe.Dal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe.Xunit;

public class RecipeDto
{
    public string Title { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public List<string> Directions { get; set; } = new List<string>();
    public string Category { get; set; }
    public long Id { get; internal set; }
}
