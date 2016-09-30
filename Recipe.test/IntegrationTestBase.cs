using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Recipe.Dal;
using Recipe.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.test
{
    public class IntegrationTestBase
    {
        private RecipeContext _context;
        protected RecipeContext context
        {
            get
            {
                if (_context == null)
                {
                    _context = RecipeContext.RecipeContextFactory();
                }
                return _context;
            }
        }
    }
}
