using FluentAssertions;
using MealPlanner.Application.DTOs.Recipes;
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
    public async Task GetRecipeByIdAsync_ReturnsRecipeDto_WhenRecipeExists()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = 1,
            Name = "Chicken Pasta",
            Description = "Delicious pasta",
            ServingSize = 4,
            HouseholdId = 1,
            RecipeIngredients = new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    IngredientId = 1,
                    Ingredient = new Ingredient { Id = 1, Name = "Pasta" },
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                }
            }
        };

        _mockRecipeRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        // Act
        var result = await _recipeService.GetRecipeByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Chicken Pasta");
        result.Ingredients.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetRecipeByIdAsync_ReturnsNull_WhenRecipeDoesNotExist()
    {
        // Arrange
        _mockRecipeRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        // Act
        var result = await _recipeService.GetRecipeByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRecipesByHouseholdAsync_ReturnsRecipes()
    {
        // Arrange
        var recipes = new List<Recipe>
        {
            new Recipe { Id = 1, Name = "Recipe 1", HouseholdId = 1, ServingSize = 2 },
            new Recipe { Id = 2, Name = "Recipe 2", HouseholdId = 1, ServingSize = 4 }
        };

        _mockRecipeRepository
            .Setup(r => r.GetByHouseholdIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        // Act
        var result = await _recipeService.GetRecipesByHouseholdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateRecipeAsync_CreatesRecipe()
    {
        // Arrange
        var dto = new SaveRecipeDto
        {
            Name = "New Recipe",
            Description = "Test recipe",
            Instructions = "Cook it",
            PrepTimeMinutes = 10,
            CookTimeMinutes = 20,
            ServingSize = 4,
            Ingredients = new List<SaveRecipeIngredientDto>
            {
                new SaveRecipeIngredientDto
                {
                    IngredientId = 1,
                    Quantity = 200,
                    Unit = (int)MeasurementUnit.Gram
                }
            }
        };

        var createdRecipe = new Recipe
        {
            Id = 1,
            Name = "New Recipe",
            Description = "Test recipe",
            Instructions = "Cook it",
            PrepTimeMinutes = 10,
            CookTimeMinutes = 20,
            ServingSize = 4,
            HouseholdId = 100,
            RecipeIngredients = new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    IngredientId = 1,
                    Ingredient = new Ingredient { Id = 1, Name = "Pasta" },
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                }
            }
        };

        _mockRecipeRepository
            .Setup(r => r.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdRecipe);

        // Act
        var result = await _recipeService.CreateRecipeAsync(100, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Recipe");
        result.HouseholdId.Should().Be(100);
        result.Ingredients.Should().HaveCount(1);
        _mockRecipeRepository.Verify(r => r.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRecipeAsync_UpdatesRecipe()
    {
        // Arrange
        var existingRecipe = new Recipe
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Description",
            ServingSize = 2,
            HouseholdId = 1,
            RecipeIngredients = new List<RecipeIngredient>()
        };

        var dto = new SaveRecipeDto
        {
            Name = "New Name",
            Description = "New Description",
            Instructions = "New Instructions",
            PrepTimeMinutes = 15,
            CookTimeMinutes = 25,
            ServingSize = 4,
            Ingredients = new List<SaveRecipeIngredientDto>()
        };

        _mockRecipeRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecipe);

        // Act
        await _recipeService.UpdateRecipeAsync(1, dto);

        // Assert
        existingRecipe.Name.Should().Be("New Name");
        existingRecipe.Description.Should().Be("New Description");
        existingRecipe.ServingSize.Should().Be(4);
        _mockRecipeRepository.Verify(r => r.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRecipeAsync_DeletesRecipe()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = 1,
            Name = "Recipe to Delete",
            HouseholdId = 1,
            ServingSize = 2
        };

        _mockRecipeRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        // Act
        await _recipeService.DeleteRecipeAsync(1);

        // Assert
        _mockRecipeRepository.Verify(r => r.DeleteAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRecipeAsync_ThrowsException_WhenRecipeNotFound()
    {
        // Arrange
        _mockRecipeRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        // Act
        Func<Task> act = async () => await _recipeService.DeleteRecipeAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Recipe with ID 999 not found");
    }

    [Fact]
    public async Task SearchRecipesAsync_ReturnsMatchingRecipes()
    {
        // Arrange
        var recipes = new List<Recipe>
        {
            new Recipe { Id = 1, Name = "Chicken Pasta", HouseholdId = 1, ServingSize = 2 },
            new Recipe { Id = 2, Name = "Chicken Soup", HouseholdId = 1, ServingSize = 4 }
        };

        _mockRecipeRepository
            .Setup(r => r.SearchByNameAsync(1, "chicken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        // Act
        var result = await _recipeService.SearchRecipesAsync(1, "chicken");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.Name.Should().Contain("Chicken"));
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
        result.Ingredients.ElementAt(0).Quantity.Should().Be(1000); // 500 * 2
        result.Ingredients.ElementAt(1).Quantity.Should().Be(400);  // 200 * 2
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
        result.Ingredients.ElementAt(0).Quantity.Should().Be(200); // 400 / 2
        result.Ingredients.ElementAt(1).Quantity.Should().Be(2);   // 4 / 2
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
