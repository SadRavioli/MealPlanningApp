using FluentAssertions;
using MealPlanner.Application.DTOs.ShoppingLists;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;
using Moq;

namespace MealPlanner.Application.Tests.Services;

public class ShoppingListServiceTests
{
    private readonly Mock<IShoppingListRepository> _mockShoppingListRepository;
    private readonly Mock<IMealPlanRepository> _mockMealPlanRepository;
    private readonly ShoppingListService _shoppingListService;

    public ShoppingListServiceTests()
    {
        _mockShoppingListRepository = new Mock<IShoppingListRepository>();
        _mockMealPlanRepository = new Mock<IMealPlanRepository>();
        _shoppingListService = new ShoppingListService(
            _mockShoppingListRepository.Object,
            _mockMealPlanRepository.Object);
    }

    [Fact]
    public async Task GetShoppingListByIdAsync_ReturnsShoppingListDto_WhenShoppingListExists()
    {
        // Arrange
        var shoppingList = new ShoppingList
        {
            Id = 1,
            HouseholdId = 1,
            MealPlanId = 1,
            CreatedAt = DateTime.UtcNow,
            Items = new List<ShoppingListItem>
            {
                new ShoppingListItem
                {
                    Id = 1,
                    IngredientId = 1,
                    Ingredient = new Ingredient { Id = 1, Name = "Chicken" },
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram,
                    IsChecked = false
                }
            }
        };

        _mockShoppingListRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        // Act
        var result = await _shoppingListService.GetShoppingListByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GenerateFromMealPlanAsync_AggregatesIngredientsFromMealPlan()
    {
        // Arrange
        var mealPlan = new MealPlan
        {
            Id = 1,
            HouseholdId = 1,
            WeekStartDate = new DateTime(2025, 1, 6),
            PlannedMeals = new List<PlannedMeal>
            {
                new PlannedMeal
                {
                    Id = 1,
                    RecipeId = 1,
                    Servings = 4,
                    Recipe = new Recipe
                    {
                        Id = 1,
                        Name = "Pasta",
                        ServingSize = 2,
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient
                            {
                                IngredientId = 1,
                                Ingredient = new Ingredient { Id = 1, Name = "Pasta" },
                                Quantity = 200,
                                Unit = MeasurementUnit.Gram
                            },
                            new RecipeIngredient
                            {
                                IngredientId = 2,
                                Ingredient = new Ingredient { Id = 2, Name = "Tomato" },
                                Quantity = 100,
                                Unit = MeasurementUnit.Gram
                            }
                        }
                    }
                },
                new PlannedMeal
                {
                    Id = 2,
                    RecipeId = 2,
                    Servings = 2,
                    Recipe = new Recipe
                    {
                        Id = 2,
                        Name = "Salad",
                        ServingSize = 2,
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient
                            {
                                IngredientId = 2,
                                Ingredient = new Ingredient { Id = 2, Name = "Tomato" },
                                Quantity = 50,
                                Unit = MeasurementUnit.Gram
                            }
                        }
                    }
                }
            }
        };

        var capturedShoppingList = (ShoppingList?)null;

        _mockMealPlanRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mealPlan);

        _mockShoppingListRepository
            .Setup(r => r.AddAsync(It.IsAny<ShoppingList>(), It.IsAny<CancellationToken>()))
            .Callback<ShoppingList, CancellationToken>((sl, ct) => capturedShoppingList = sl)
            .ReturnsAsync((ShoppingList sl, CancellationToken ct) =>
            {
                sl.Id = 1;
                return sl;
            });

        // Act
        var result = await _shoppingListService.GenerateFromMealPlanAsync(mealPlanId: 1, householdId: 1);

        // Assert
        result.Should().NotBeNull();
        capturedShoppingList.Should().NotBeNull();
        capturedShoppingList!.Items.Should().HaveCount(2); // Pasta and Tomato

        // Pasta: 200g * (4 servings / 2 serving size) = 400g
        var pastaItem = capturedShoppingList.Items.FirstOrDefault(i => i.IngredientId == 1);
        pastaItem.Should().NotBeNull();
        pastaItem!.Quantity.Should().Be(400);

        // Tomato: (100g * (4/2)) + (50g * (2/2)) = 200g + 50g = 250g
        var tomatoItem = capturedShoppingList.Items.FirstOrDefault(i => i.IngredientId == 2);
        tomatoItem.Should().NotBeNull();
        tomatoItem!.Quantity.Should().Be(250);
    }

    [Fact]
    public async Task GenerateFromMealPlanAsync_ThrowsException_WhenMealPlanNotFound()
    {
        // Arrange
        _mockMealPlanRepository
            .Setup(r => r.GetByIdWithIngredientsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MealPlan?)null);

        // Act
        Func<Task> act = async () => await _shoppingListService.GenerateFromMealPlanAsync(999, householdId: 1);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Meal plan with ID 999 not found");
    }

    [Fact]
    public async Task CreateShoppingListAsync_CreatesShoppingList()
    {
        // Arrange
        var dto = new SaveShoppingListDto
        {
            MealPlanId = 1,
            Notes = "Weekly groceries",
            Items = new List<SaveShoppingListItemDto>
            {
                new SaveShoppingListItemDto
                {
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram
                }
            }
        };

        var createdShoppingList = new ShoppingList
        {
            Id = 1,
            HouseholdId = 100,
            MealPlanId = 1,
            Notes = "Weekly groceries",
            CreatedAt = DateTime.UtcNow,
            Items = new List<ShoppingListItem>
            {
                new ShoppingListItem
                {
                    Id = 1,
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram,
                    IsChecked = false
                }
            }
        };

        _mockShoppingListRepository
            .Setup(r => r.AddAsync(It.IsAny<ShoppingList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdShoppingList);

        // Act
        var result = await _shoppingListService.CreateShoppingListAsync(dto, householdId: 100);

        // Assert
        result.Should().NotBeNull();
        result.HouseholdId.Should().Be(100);
        result.Items.Should().HaveCount(1);
        _mockShoppingListRepository.Verify(r => r.AddAsync(It.IsAny<ShoppingList>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleItemCheckedAsync_TogglesCheckedStatus()
    {
        // Arrange
        var shoppingList = new ShoppingList
        {
            Id = 1,
            HouseholdId = 1,
            CreatedAt = DateTime.UtcNow,
            Items = new List<ShoppingListItem>
            {
                new ShoppingListItem
                {
                    Id = 1,
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram,
                    IsChecked = false
                }
            }
        };

        _mockShoppingListRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        // Act
        await _shoppingListService.ToggleItemCheckedAsync(shoppingListId: 1, itemId: 1);

        // Assert
        shoppingList.Items.First().IsChecked.Should().BeTrue();
        _mockShoppingListRepository.Verify(r => r.UpdateAsync(It.IsAny<ShoppingList>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteShoppingListAsync_ThrowsException_WhenShoppingListNotFound()
    {
        // Arrange
        _mockShoppingListRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingList?)null);

        // Act
        Func<Task> act = async () => await _shoppingListService.DeleteShoppingListAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Shopping list with ID 999 not found");
    }
}
