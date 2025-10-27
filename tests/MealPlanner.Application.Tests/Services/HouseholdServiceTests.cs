using FluentAssertions;
using MealPlanner.Application.DTOs.Households;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;
using Moq;

namespace MealPlanner.Application.Tests.Services;

public class HouseholdServiceTests
{
    private readonly Mock<IHouseholdRepository> _mockHouseholdRepository;
    private readonly HouseholdService _householdService;

    public HouseholdServiceTests()
    {
        _mockHouseholdRepository = new Mock<IHouseholdRepository>();
        _householdService = new HouseholdService(_mockHouseholdRepository.Object);
    }

    [Fact]
    public async Task GetHouseholdByIdAsync_ReturnsHouseholdDto_WhenHouseholdExists()
    {
        // Arrange
        var household = new Household
        {
            Id = 1,
            Name = "Smith Family",
            CreatedAt = DateTime.UtcNow,
            UserHouseholds = new List<UserHousehold>
            {
                new UserHousehold
                {
                    UserId = "user-123",
                    HouseholdId = 1,
                    Role = HouseholdRole.Admin,
                    JoinedAt = DateTime.UtcNow
                }
            }
        };

        _mockHouseholdRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);

        // Act
        var result = await _householdService.GetHouseholdByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Smith Family");
        result.Members.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetHouseholdsByUserAsync_ReturnsUserHouseholds()
    {
        // Arrange
        var households = new List<Household>
        {
            new Household
            {
                Id = 1,
                Name = "Smith Family",
                CreatedAt = DateTime.UtcNow,
                UserHouseholds = new List<UserHousehold>()
            },
            new Household
            {
                Id = 2,
                Name = "Work Group",
                CreatedAt = DateTime.UtcNow,
                UserHouseholds = new List<UserHousehold>()
            }
        };

        _mockHouseholdRepository
            .Setup(r => r.GetByUserIdAsync("user-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(households);

        // Act
        var result = await _householdService.GetHouseholdsByUserAsync("user-123");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateHouseholdAsync_CreatesHousehold()
    {
        // Arrange
        var dto = new SaveHouseholdDto
        {
            Name = "Johnson Family"
        };

        var createdHousehold = new Household
        {
            Id = 1,
            Name = "Johnson Family",
            CreatedAt = DateTime.UtcNow,
            UserHouseholds = new List<UserHousehold>()
        };

        _mockHouseholdRepository
            .Setup(r => r.AddAsync(It.IsAny<Household>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdHousehold);

        // Act
        var result = await _householdService.CreateHouseholdAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Johnson Family");
        _mockHouseholdRepository.Verify(r => r.AddAsync(It.IsAny<Household>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddMemberToHouseholdAsync_AddsMember()
    {
        // Arrange
        var household = new Household
        {
            Id = 1,
            Name = "Smith Family",
            CreatedAt = DateTime.UtcNow,
            UserHouseholds = new List<UserHousehold>()
        };

        var memberDto = new HouseholdMemberDto
        {
            UserId = "user-456",
            Role = HouseholdRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        _mockHouseholdRepository
            .Setup(r => r.GetByIdWithMembersAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);

        // Act
        await _householdService.AddMemberToHouseholdAsync(1, memberDto);

        // Assert
        household.UserHouseholds.Should().HaveCount(1);
        household.UserHouseholds.First().UserId.Should().Be("user-456");
        household.UserHouseholds.First().Role.Should().Be(HouseholdRole.Member);
        _mockHouseholdRepository.Verify(r => r.UpdateAsync(It.IsAny<Household>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveMemberFromHouseholdAsync_RemovesMember()
    {
        // Arrange
        var household = new Household
        {
            Id = 1,
            Name = "Smith Family",
            CreatedAt = DateTime.UtcNow,
            UserHouseholds = new List<UserHousehold>
            {
                new UserHousehold
                {
                    UserId = "user-123",
                    HouseholdId = 1,
                    Role = HouseholdRole.Admin,
                    JoinedAt = DateTime.UtcNow
                },
                new UserHousehold
                {
                    UserId = "user-456",
                    HouseholdId = 1,
                    Role = HouseholdRole.Member,
                    JoinedAt = DateTime.UtcNow
                }
            }
        };

        _mockHouseholdRepository
            .Setup(r => r.GetByIdWithMembersAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);

        // Act
        await _householdService.RemoveMemberFromHouseholdAsync(1, "user-456");

        // Assert
        household.UserHouseholds.Should().HaveCount(1);
        household.UserHouseholds.Should().NotContain(uh => uh.UserId == "user-456");
        _mockHouseholdRepository.Verify(r => r.UpdateAsync(It.IsAny<Household>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHouseholdMemberRoleAsync_UpdatesRole()
    {
        // Arrange
        var household = new Household
        {
            Id = 1,
            Name = "Smith Family",
            CreatedAt = DateTime.UtcNow,
            UserHouseholds = new List<UserHousehold>
            {
                new UserHousehold
                {
                    UserId = "user-456",
                    HouseholdId = 1,
                    Role = HouseholdRole.Member,
                    JoinedAt = DateTime.UtcNow
                }
            }
        };

        _mockHouseholdRepository
            .Setup(r => r.GetByIdWithMembersAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);

        // Act
        await _householdService.UpdateHouseholdMemberRoleAsync(1, "user-456", HouseholdRole.Admin);

        // Assert
        household.UserHouseholds.First().Role.Should().Be(HouseholdRole.Admin);
        _mockHouseholdRepository.Verify(r => r.UpdateAsync(It.IsAny<Household>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHouseholdAsync_ThrowsException_WhenHouseholdNotFound()
    {
        // Arrange
        _mockHouseholdRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Household?)null);

        // Act
        Func<Task> act = async () => await _householdService.DeleteHouseholdAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Household with ID 999 not found");
    }
}
