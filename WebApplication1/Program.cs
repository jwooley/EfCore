using Microsoft.EntityFrameworkCore;
using Recipe.Dal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

const string ConnectionString = "data source=.;initial catalog=recipecore;integrated security=true;TrustServerCertificate=true;";
builder.Services.AddDbContextPool<RecipeContext>(options =>
    options.UseSqlServer(ConnectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
