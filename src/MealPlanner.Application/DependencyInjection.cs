using FluentValidation;
using MealPlanner.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators - scans assembly for all validators
        services.AddValidatorsFromAssemblyContaining<IRecipeService>();

        // Register application services
        services.AddScoped<IRecipeService, RecipeService>();

        // TODO: Add other services as you create them:
        // services.AddScoped<IMealPlanService, MealPlanService>();
        // services.AddScoped<IIngredientService, IngredientService>();
        // services.AddScoped<IHouseholdService, HouseholdService>();

        return services;
    }
}
