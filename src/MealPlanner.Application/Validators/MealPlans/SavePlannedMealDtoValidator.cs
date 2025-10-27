using FluentValidation;
using MealPlanner.Application.DTOs.MealPlans;

namespace MealPlanner.Application.Validators.MealPlans;

public class SavePlannedMealDtoValidator : AbstractValidator<SavePlannedMealDto>
{
    public SavePlannedMealDtoValidator()
    {
        RuleFor(x => x.RecipeId)
            .GreaterThan(0).WithMessage("Recipe ID must be valid");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Day of week must be valid");

        RuleFor(x => x.MealType)
            .IsInEnum().WithMessage("Meal type must be valid");

        RuleFor(x => x.Servings)
            .GreaterThan(0).WithMessage("Servings must be greater than zero");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    }
}
