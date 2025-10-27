using FluentAssertions;
using MealPlanner.Application.DTOs.Pantries;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;
using Moq;

namespace MealPlanner.Application.Tests.Services;

public class PantryServiceTests
{
    private readonly Mock<IPantryRepository> _mockPantryRepository;
    private readonly PantryService _pantryService;

    public PantryServiceTests()
    {
        _mockPantryRepository = new Mock<IPantryRepository>();
        _pantryService = new PantryService(_mockPantryRepository.Object);
    }

    [Fact]
    public async Task GetPantryByHouseholdIdAsync_ReturnsPantryDto_WhenPantryExists()
    {
        // Arrange
        var pantry = new Pantry
        {
            Id = 1,
            HouseholdId = 1,
            Items = new List<PantryItem>
            {
                new PantryItem
                {
                    Id = 1,
                    PantryId = 1,
                    IngredientId = 1,
                    Ingredient = new Ingredient { Id = 1, Name = "Rice" },
                    Quantity = 1000,
                    Unit = MeasurementUnit.Gram,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    AddedAt = DateTime.UtcNow
                }
            }
        };

        _mockPantryRepository
            .Setup(r => r.GetByHouseholdIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        // Act
        var result = await _pantryService.GetPantryByHouseholdIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.HouseholdId.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPantryByHouseholdIdAsync_ReturnsNull_WhenPantryDoesNotExist()
    {
        // Arrange
        _mockPantryRepository
            .Setup(r => r.GetByHouseholdIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pantry?)null);

        // Act
        var result = await _pantryService.GetPantryByHouseholdIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreatePantryAsync_CreatesPantry()
    {
        // Arrange
        var createdPantry = new Pantry
        {
            Id = 1,
            HouseholdId = 100,
            Items = new List<PantryItem>()
        };

        _mockPantryRepository
            .Setup(r => r.AddAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdPantry);

        // Act
        var result = await _pantryService.CreatePantryAsync(householdId: 100);

        // Assert
        result.Should().NotBeNull();
        result.HouseholdId.Should().Be(100);
        _mockPantryRepository.Verify(r => r.AddAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddItemToPantryAsync_CreatesNewPantry_WhenPantryDoesNotExist()
    {
        // Arrange
        var dto = new SavePantryItemDto
        {
            IngredientId = 1,
            Quantity = 500,
            Unit = MeasurementUnit.Gram,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        var createdPantry = new Pantry
        {
            Id = 1,
            HouseholdId = 100,
            Items = new List<PantryItem>()
        };

        _mockPantryRepository
            .Setup(r => r.GetByHouseholdIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pantry?)null);

        _mockPantryRepository
            .Setup(r => r.AddAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdPantry);

        _mockPantryRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdPantry);

        // Act
        var result = await _pantryService.AddItemToPantryAsync(householdId: 100, dto);

        // Assert
        result.Should().NotBeNull();
        result.IngredientId.Should().Be(1);
        result.Quantity.Should().Be(500);
        _mockPantryRepository.Verify(r => r.AddAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockPantryRepository.Verify(r => r.UpdateAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddItemToPantryAsync_AddsItemToExistingPantry()
    {
        // Arrange
        var pantry = new Pantry
        {
            Id = 1,
            HouseholdId = 100,
            Items = new List<PantryItem>()
        };

        var dto = new SavePantryItemDto
        {
            IngredientId = 1,
            Quantity = 500,
            Unit = MeasurementUnit.Gram,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        _mockPantryRepository
            .Setup(r => r.GetByHouseholdIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        _mockPantryRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        // Act
        var result = await _pantryService.AddItemToPantryAsync(householdId: 100, dto);

        // Assert
        result.Should().NotBeNull();
        result.IngredientId.Should().Be(1);
        result.Quantity.Should().Be(500);
        pantry.Items.Should().HaveCount(1);
        _mockPantryRepository.Verify(r => r.AddAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockPantryRepository.Verify(r => r.UpdateAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePantryItemAsync_UpdatesItem()
    {
        // Arrange
        var pantry = new Pantry
        {
            Id = 1,
            HouseholdId = 100,
            Items = new List<PantryItem>
            {
                new PantryItem
                {
                    Id = 1,
                    PantryId = 1,
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram,
                    ExpiryDate = DateTime.UtcNow.AddDays(7)
                }
            }
        };

        var dto = new SavePantryItemDto
        {
            IngredientId = 1,
            Quantity = 750,
            Unit = MeasurementUnit.Gram,
            ExpiryDate = DateTime.UtcNow.AddDays(14)
        };

        _mockPantryRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        // Act
        await _pantryService.UpdatePantryItemAsync(pantryId: 1, itemId: 1, dto);

        // Assert
        var item = pantry.Items.First();
        item.Quantity.Should().Be(750);
        item.ExpiryDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(14), TimeSpan.FromSeconds(1));
        _mockPantryRepository.Verify(r => r.UpdateAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveItemFromPantryAsync_RemovesItem()
    {
        // Arrange
        var pantry = new Pantry
        {
            Id = 1,
            HouseholdId = 100,
            Items = new List<PantryItem>
            {
                new PantryItem
                {
                    Id = 1,
                    PantryId = 1,
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram
                },
                new PantryItem
                {
                    Id = 2,
                    PantryId = 1,
                    IngredientId = 2,
                    Quantity = 300,
                    Unit = MeasurementUnit.Gram
                }
            }
        };

        _mockPantryRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        // Act
        await _pantryService.RemoveItemFromPantryAsync(pantryId: 1, itemId: 1);

        // Assert
        pantry.Items.Should().HaveCount(1);
        pantry.Items.Should().NotContain(i => i.Id == 1);
        _mockPantryRepository.Verify(r => r.UpdateAsync(It.IsAny<Pantry>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePantryItemAsync_ThrowsException_WhenPantryNotFound()
    {
        // Arrange
        var dto = new SavePantryItemDto
        {
            IngredientId = 1,
            Quantity = 500,
            Unit = MeasurementUnit.Gram
        };

        _mockPantryRepository
            .Setup(r => r.GetByIdWithItemsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pantry?)null);

        // Act
        Func<Task> act = async () => await _pantryService.UpdatePantryItemAsync(999, itemId: 1, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pantry with ID 999 not found");
    }

    [Fact]
    public async Task RemoveItemFromPantryAsync_ThrowsException_WhenItemNotFound()
    {
        // Arrange
        var pantry = new Pantry
        {
            Id = 1,
            HouseholdId = 100,
            Items = new List<PantryItem>()
        };

        _mockPantryRepository
            .Setup(r => r.GetByIdWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        // Act
        Func<Task> act = async () => await _pantryService.RemoveItemFromPantryAsync(pantryId: 1, itemId: 999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pantry item with ID 999 not found");
    }
}
