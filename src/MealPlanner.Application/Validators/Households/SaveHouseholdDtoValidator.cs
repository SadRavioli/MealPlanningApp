using FluentValidation;
using MealPlanner.Application.DTOs.Households;

namespace MealPlanner.Application.Validators.Households;

public class SaveHouseholdDtoValidator : AbstractValidator<SaveHouseholdDto>
{
    public SaveHouseholdDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Household name is required")
            .MaximumLength(100).WithMessage("Household name cannot exceed 100 characters");
    }
}
