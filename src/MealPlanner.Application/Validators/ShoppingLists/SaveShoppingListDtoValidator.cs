using FluentValidation;
using MealPlanner.Application.DTOs.ShoppingLists;

namespace MealPlanner.Application.Validators.ShoppingLists;

public class SaveShoppingListDtoValidator : AbstractValidator<SaveShoppingListDto>
{
    public SaveShoppingListDtoValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items list cannot be null");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.IngredientId)
                    .GreaterThan(0).WithMessage("Ingredient ID must be greater than 0");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            });
    }
}
