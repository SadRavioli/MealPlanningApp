using FluentValidation;
using MealPlanner.Application.DTOs.MealPlans;

namespace MealPlanner.Application.Validators.MealPlans;

public class SaveMealPlanDtoValidator : AbstractValidator<SaveMealPlanDto>
{
    public SaveMealPlanDtoValidator()
    {
        RuleFor(x => x.WeekStartDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Week start date must be in the future");

        RuleFor(x => x.PlannedMeals)
            .NotEmpty().WithMessage("Meal plan must have at least one planned meal");

        RuleForEach(x => x.PlannedMeals)
            .SetValidator(new SavePlannedMealDtoValidator());
    }
}
