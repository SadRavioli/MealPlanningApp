using FluentValidation;
using MealPlanner.Application.DTOs.Recipes;

namespace MealPlanner.Application.Validators.Recipes;

public class SaveRecipeIngredientDtoValidator : AbstractValidator<SaveRecipeIngredientDto>
{
    public SaveRecipeIngredientDtoValidator()
    {
        RuleFor(x => x.IngredientId)
            .GreaterThan(0).WithMessage("Ingredient ID must be valid");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid measurement unit");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
