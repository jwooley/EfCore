using System;
using System.Linq;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var dc = new RecipeContext())
            {
                foreach(var recipe in dc.Recipes.Take(5))
                {

                    Console.WriteLine(recipe.Title);
                }
            }
        }
    }
}
