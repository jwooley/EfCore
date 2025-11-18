using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe.Xunit;

internal class RecipeConditions
{
    public string Category { get; internal set; }
    public string IngredientName { get; set; }
    public int Servings { get; set; }
    public string Title { get; set; }
}