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
    /// <summary>
    /// Base class for testing with in memory context
    /// </summary>
    /// <remarks>
    /// See https://docs.efproject.net/en/latest/miscellaneous/testing.html 
    /// for instructions on creating unit tests with the in memory provider.
    /// </remarks>
    public abstract class InMemoryTestBase
    {
        private RecipeContext _context;
        protected RecipeContext context
        {
            get
            {
                if (_context == null)
                {
                    _context = new RecipeContext(CreateContextOptions());
                }
                return _context;
            }
        }
        protected DbContextOptions<RecipeContext> CreateContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<RecipeContext>();
            builder.UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);
            return builder.Options;
        }
    }
}
