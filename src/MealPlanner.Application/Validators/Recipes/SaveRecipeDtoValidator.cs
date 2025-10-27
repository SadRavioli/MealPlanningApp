using FluentValidation;
using MealPlanner.Application.DTOs;

namespace MealPlanner.Application.Validators;

public class SaveRecipeDtoValidator : AbstractValidator<SaveRecipeDto>
{
    public SaveRecipeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Recipe name is required")
            .MaximumLength(200).WithMessage("Recipe name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Instructions)
            .MaximumLength(5000).WithMessage("Instructions cannot exceed 5000 characters");

        RuleFor(x => x.PrepTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time can't be less than zero");

        RuleFor(x => x.CookTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Cook time can't be less than zero");

        RuleFor(x => x.ServingSize)
            .GreaterThan(0).WithMessage("Serving size must be greater than zero");

        // Example: Validate nested ingredients collection
        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("Recipe must have at least one ingredient");

        RuleForEach(x => x.Ingredients)
            .SetValidator(new SaveRecipeIngredientDtoValidator());
    }
}
