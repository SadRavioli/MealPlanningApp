using MealPlanner.Domain.Enums;

namespace MealPlanner.Web.Extensions;

public static class MeasurementUnitExtensions
{
    public static string ToAbbreviation(this MeasurementUnit unit)
    {
        return unit switch
        {
            // Weight
            MeasurementUnit.Gram => "g",
            MeasurementUnit.Kilogram => "kg",
            MeasurementUnit.Ounce => "oz",
            MeasurementUnit.Pound => "lb",

            // Volume
            MeasurementUnit.Millilitre => "ml",
            MeasurementUnit.Litre => "L",
            MeasurementUnit.Teaspoon => "tsp",
            MeasurementUnit.Tablespoon => "tbsp",
            MeasurementUnit.Cup => "cup",
            MeasurementUnit.Pint => "pt",
            MeasurementUnit.Quart => "qt",
            MeasurementUnit.Gallon => "gal",
            MeasurementUnit.FluidOunce => "fl oz",

            // Count
            MeasurementUnit.Piece => "piece",
            MeasurementUnit.Whole => "whole",
            MeasurementUnit.Clove => "clove",
            MeasurementUnit.Slice => "slice",
            MeasurementUnit.Pinch => "pinch",
            MeasurementUnit.Dash => "dash",

            // Other
            MeasurementUnit.ToTaste => "to taste",

            _ => unit.ToString()
        };
    }

    public static string FormatQuantity(this decimal quantity)
    {
        // Format: 500.0 → "500", 1.5 → "1.5", 0.25 → "0.25"
        return quantity.ToString("0.##");
    }

    public static string FormatWithUnit(this decimal quantity, MeasurementUnit unit)
    {
        var formattedQuantity = quantity.FormatQuantity();
        var abbreviation = unit.ToAbbreviation();

        // Special case: "to taste" doesn't need quantity
        if (abbreviation == "to taste")
            return abbreviation;

        // Word-based units need space (some pluralize, some don't)
        var wordUnits = new[] { "piece", "whole", "clove", "slice", "pinch", "dash" };

        if (wordUnits.Contains(abbreviation))
        {
            var unitText = quantity == 1 ? abbreviation : Pluralize(abbreviation);
            return $"{formattedQuantity} {unitText}";
        }

        // Abbreviated units: no space, no pluralization
        return $"{formattedQuantity}{abbreviation}";
    }

    private static string Pluralize(string word)
    {
        return word switch
        {
            "whole" => "whole",
            "piece" => "pieces",
            "clove" => "cloves",
            "slice" => "slices",
            "pinch" => "pinches",
            "dash" => "dashes",
            _ => word + "s"
        };
    }
}