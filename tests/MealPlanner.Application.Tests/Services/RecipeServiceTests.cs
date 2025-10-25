using FluentAssertions;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;
using Moq;

namespace MealPlanner.Application.Tests.Services;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly RecipeService _recipeService;

    public RecipeServiceTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _recipeService = new RecipeService(_mockRecipeRepository.Object);
    }

    [Fact]
    public async Task ScaleRecipe_DoublesIngredientQuantities_WhenServingsAreDoubled()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = 1,
            Name = "Chicken Pasta",
            ServingSize = 2,
            HouseholdId = 1,
            RecipeIngredients = new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    IngredientId = 1,
                    Ingredient = new Ingredient { Id = 1, Name = "Chicken Breast" },
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram
                },
                new RecipeIngredient
                {
                    IngredientId = 2,
                    Ingredient = new Ingredient { Id = 2, Name = "Pasta" },
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                }
            }
        };

        _mockRecipeRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        // Act
        var result = await _recipeService.ScaleRecipeAsync(1, newServings: 4);

        // Assert
        result.Should().NotBeNull();
        result.ServingSize.Should().Be(4);
        result.Ingredients.Should().HaveCount(2);
        result.Ingredients[0].Quantity.Should().Be(1000); // 500 * 2
        result.Ingredients[1].Quantity.Should().Be(400);  // 200 * 2
    }

    [Fact]
    public async Task ScaleRecipe_HalvesIngredientQuantities_WhenServingsAreHalved()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = 1,
            Name = "Pasta Sauce",
            ServingSize = 4,
            HouseholdId = 1,
            RecipeIngredients = new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    IngredientId = 1,
                    Ingredient = new Ingredient { Id = 1, Name = "Tomatoes" },
                    Quantity = 400,
                    Unit = MeasurementUnit.Gram
                },
                new RecipeIngredient
                {
                    IngredientId = 2,
                    Ingredient = new Ingredient { Id = 2, Name = "Garlic" },
                    Quantity = 4,
                    Unit = MeasurementUnit.Clove
                }
            }
        };

        _mockRecipeRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        // Act
        var result = await _recipeService.ScaleRecipeAsync(1, newServings: 2);

        // Assert
        result.Should().NotBeNull();
        result.ServingSize.Should().Be(2);
        result.Ingredients[0].Quantity.Should().Be(200); // 400 / 2
        result.Ingredients[1].Quantity.Should().Be(2);   // 4 / 2
    }

    [Fact]
    public async Task ScaleRecipe_ThrowsException_WhenRecipeNotFound()
    {
        // Arrange
        _mockRecipeRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        // Act
        Func<Task> act = async () => await _recipeService.ScaleRecipeAsync(999, newServings: 4);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Recipe with ID 999 not found");
    }

    [Fact]
    public async Task ScaleRecipe_ThrowsException_WhenNewServingsIsZeroOrNegative()
    {
        // Arrange
        var recipe = new Recipe { Id = 1, Name = "Test", ServingSize = 2, HouseholdId = 1 };
        _mockRecipeRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        // Act
        Func<Task> act = async () => await _recipeService.ScaleRecipeAsync(1, newServings: 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*servings must be greater than zero*");
    }
}
