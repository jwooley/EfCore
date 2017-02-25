using System;
using Recipe.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dc = new RecipeContext())
            {
                foreach(var recipe in dc.Recipes.Include(r => r.Direction).Take(5))
                {
                    Console.WriteLine(recipe.Title);
                    foreach(var direction in recipe.Direction)
                    {
                        Console.WriteLine(direction.Description);
                    }
                    Console.WriteLine("-----");
                }
            }
        }
    }
}