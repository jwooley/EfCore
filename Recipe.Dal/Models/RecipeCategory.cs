namespace Recipe.Dal.Models;

public partial class RecipeCategory
{
    public long RecipeId { get; set; }
    public long CategoryId { get; set; }

    public virtual Category Category { get; set; }
    public virtual RecipeOld Recipe { get; set; }
}
