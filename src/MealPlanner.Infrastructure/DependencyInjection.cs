using MealPlanner.Application.Services;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using MealPlanner.Infrastructure.Repositories;
using MealPlanner.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Handle Railway DATABASE_URL if present
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres://"))
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=True";
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Use SQLite for local development, PostgreSQL for production
            if (connectionString.Contains("Host=") || connectionString.Contains("Server="))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IHouseholdRepository, HouseholdRepository>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IPantryRepository, PantryRepository>();

        // Services that depend on Infrastructure (Identity)
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
