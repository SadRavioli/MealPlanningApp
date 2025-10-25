using MealPlanner.Domain.Enums;

namespace MealPlanner.Domain.ValueObjects;

public record Quantity(decimal Amount, MeasurementUnit Unit)
{
    public Quantity Scale(decimal factor) => this with { Amount = Amount * factor };

    public override string ToString() => $"{Amount} {Unit}";
}
