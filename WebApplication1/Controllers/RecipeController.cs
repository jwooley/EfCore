using Microsoft.AspNetCore.Mvc;
using Recipe.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class RecipeController : Controller
    {
        private RecipeContext _Context;

        public RecipeController(RecipeContext context)
        {
            _Context = context;
        }

        [HttpGet]
        [Route("api/Recipe/Get")]
        public IQueryable<Recipe.Dal.Models.Recipe> GetRecipes()
        {
            return _Context.Recipes.Take(50);
        }

        [HttpGet]
        [Route("api/Recipe/Get/{id}")]
        public ActionResult<Recipe.Dal.Models.Recipe?> GetRecipe(long id)
        {
            return _Context.Recipes.Find(id);
        }

        [HttpPost]
        [Route("api/Recipe")]
        public ActionResult UpsertRecipe(Recipe.Dal.Models.Recipe recipe)
        {
            if (recipe.Id == 0)
            {
                _Context.Recipes.Add(recipe);
                return Created($"api/Recipe/Get/{recipe.Id}", recipe);
            }
            else
            {
                _Context.Recipes.Update(recipe);
                return Ok(recipe.Id);
            }
        }
    }
}
