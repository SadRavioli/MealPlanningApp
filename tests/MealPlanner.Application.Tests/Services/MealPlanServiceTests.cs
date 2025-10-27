using FluentAssertions;
using MealPlanner.Application.DTOs.MealPlans;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;
using Moq;

namespace MealPlanner.Application.Tests.Services;

public class MealPlanServiceTests
{
    private readonly Mock<IMealPlanRepository> _mockMealPlanRepository;
    private readonly MealPlanService _mealPlanService;

    public MealPlanServiceTests()
    {
        _mockMealPlanRepository = new Mock<IMealPlanRepository>();
        _mealPlanService = new MealPlanService(_mockMealPlanRepository.Object);
    }

    [Fact]
    public async Task GetMealPlanByIdAsync_ReturnsMealPlanDto_WhenMealPlanExists()
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
                    Recipe = new Recipe { Id = 1, Name = "Pasta", ServingSize = 4 },
                    DayOfWeek = DayOfWeek.Monday,
                    MealType = MealType.Dinner,
                    Servings = 4
                }
            }
        };

        _mockMealPlanRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mealPlan);

        // Act
        var result = await _mealPlanService.GetMealPlanByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.HouseholdId.Should().Be(1);
        result.PlannedMeals.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMealPlanByIdAsync_ReturnsNull_WhenMealPlanDoesNotExist()
    {
        // Arrange
        _mockMealPlanRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MealPlan?)null);

        // Act
        var result = await _mealPlanService.GetMealPlanByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateMealPlanAsync_CreatesMealPlanWithPlannedMeals()
    {
        // Arrange
        var dto = new SaveMealPlanDto
        {
            WeekStartDate = new DateTime(2025, 1, 6),
            PlannedMeals = new List<SavePlannedMealDto>
            {
                new SavePlannedMealDto
                {
                    RecipeId = 1,
                    DayOfWeek = DayOfWeek.Monday,
                    MealType = MealType.Dinner,
                    Servings = 4
                }
            }
        };

        var createdMealPlan = new MealPlan
        {
            Id = 1,
            HouseholdId = 100,
            WeekStartDate = dto.WeekStartDate,
            PlannedMeals = new List<PlannedMeal>
            {
                new PlannedMeal
                {
                    Id = 1,
                    RecipeId = 1,
                    DayOfWeek = DayOfWeek.Monday,
                    MealType = MealType.Dinner,
                    Servings = 4
                }
            }
        };

        _mockMealPlanRepository
            .Setup(r => r.AddAsync(It.IsAny<MealPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMealPlan);

        // Act
        var result = await _mealPlanService.CreateMealPlanAsync(100, dto);

        // Assert
        result.Should().NotBeNull();
        result.HouseholdId.Should().Be(100);
        result.PlannedMeals.Should().HaveCount(1);
        _mockMealPlanRepository.Verify(r => r.AddAsync(It.IsAny<MealPlan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMealPlanAsync_UpdatesPlannedMeals()
    {
        // Arrange
        var existingMealPlan = new MealPlan
        {
            Id = 1,
            HouseholdId = 100,
            WeekStartDate = new DateTime(2025, 1, 6),
            PlannedMeals = new List<PlannedMeal>
            {
                new PlannedMeal
                {
                    Id = 1,
                    RecipeId = 1,
                    DayOfWeek = DayOfWeek.Monday,
                    MealType = MealType.Dinner,
                    Servings = 4
                }
            }
        };

        var dto = new SaveMealPlanDto
        {
            WeekStartDate = new DateTime(2025, 1, 6),
            PlannedMeals = new List<SavePlannedMealDto>
            {
                new SavePlannedMealDto
                {
                    RecipeId = 2,
                    DayOfWeek = DayOfWeek.Tuesday,
                    MealType = MealType.Lunch,
                    Servings = 2
                }
            }
        };

        _mockMealPlanRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMealPlan);

        // Act
        await _mealPlanService.UpdateMealPlanAsync(1, dto);

        // Assert
        _mockMealPlanRepository.Verify(r => r.UpdateAsync(It.IsAny<MealPlan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMealPlanAsync_ThrowsException_WhenMealPlanNotFound()
    {
        // Arrange
        _mockMealPlanRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MealPlan?)null);

        // Act
        Func<Task> act = async () => await _mealPlanService.DeleteMealPlanAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("MealPlan with ID 999 not found");
    }
}
