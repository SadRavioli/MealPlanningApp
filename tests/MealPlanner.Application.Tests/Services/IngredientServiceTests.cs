using FluentAssertions;
using MealPlanner.Application.DTOs.Ingredients;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using Moq;

namespace MealPlanner.Application.Tests.Services;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly IngredientService _ingredientService;

    public IngredientServiceTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _ingredientService = new IngredientService(_mockIngredientRepository.Object);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredientDto_WhenIngredientExists()
    {
        // Arrange
        var ingredient = new Ingredient
        {
            Id = 1,
            Name = "Chicken Breast",
            Category = "Meat"
        };

        _mockIngredientRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        // Act
        var result = await _ingredientService.GetIngredientByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Chicken Breast");
        result.Category.Should().Be("Meat");
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull_WhenIngredientDoesNotExist()
    {
        // Arrange
        _mockIngredientRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act
        var result = await _ingredientService.GetIngredientByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_ReturnsMatchingIngredients()
    {
        // Arrange
        var ingredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Chicken Breast", Category = "Meat" },
            new Ingredient { Id = 2, Name = "Chicken Thigh", Category = "Meat" }
        };

        _mockIngredientRepository
            .Setup(r => r.SearchByNameAsync("chicken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredients);

        // Act
        var result = await _ingredientService.SearchIngredientsByNameAsync("chicken");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(i => i.Name.Should().Contain("Chicken"));
    }

    [Fact]
    public async Task CreateIngredientAsync_CreatesIngredient()
    {
        // Arrange
        var dto = new SaveIngredientDto
        {
            Name = "Tomato",
            Category = "Vegetable"
        };

        var createdIngredient = new Ingredient
        {
            Id = 1,
            Name = "Tomato",
            Category = "Vegetable"
        };

        _mockIngredientRepository
            .Setup(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdIngredient);

        // Act
        var result = await _ingredientService.CreateIngredientAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Tomato");
        result.Category.Should().Be("Vegetable");
        _mockIngredientRepository.Verify(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateIngredientAsync_UpdatesIngredient()
    {
        // Arrange
        var existingIngredient = new Ingredient
        {
            Id = 1,
            Name = "Tomato",
            Category = "Vegetable"
        };

        var dto = new SaveIngredientDto
        {
            Name = "Cherry Tomato",
            Category = "Vegetable"
        };

        _mockIngredientRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingIngredient);

        // Act
        await _ingredientService.UpdateIngredientAsync(1, dto);

        // Assert
        existingIngredient.Name.Should().Be("Cherry Tomato");
        _mockIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteIngredientAsync_ThrowsException_WhenIngredientNotFound()
    {
        // Arrange
        _mockIngredientRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act
        Func<Task> act = async () => await _ingredientService.DeleteIngredientAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Ingredient with ID 999 not found");
    }
}
