using FluentValidation;
using MealPlanner.Application.DTOs;

namespace MealPlanner.Application.Validators;

public class SaveIngredientDtoValidator : AbstractValidator<SaveIngredientDto>
{
    public SaveIngredientDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ingredient name is required")
            .MaximumLength(50).WithMessage("Ingredient name cannot exceed 50 characters");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));
    }
}