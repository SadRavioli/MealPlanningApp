using System.Reflection;
using FluentValidation;
using MealPlanner.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators - scans this assembly for all validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register application services
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IMealPlanService, MealPlanService>();

        // TODO: Add other services as you create them:
        // services.AddScoped<IIngredientService, IngredientService>();
        // services.AddScoped<IHouseholdService, HouseholdService>();

        return services;
    }
}
