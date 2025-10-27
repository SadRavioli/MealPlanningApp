using FluentValidation;
using MealPlanner.Application.DTOs.Pantries;

namespace MealPlanner.Application.Validators.Pantries;

public class SavePantryItemDtoValidator : AbstractValidator<SavePantryItemDto>
{
    public SavePantryItemDtoValidator()
    {
        RuleFor(x => x.IngredientId)
            .GreaterThan(0).WithMessage("Ingredient ID must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future")
            .When(x => x.ExpiryDate.HasValue);
    }
}
