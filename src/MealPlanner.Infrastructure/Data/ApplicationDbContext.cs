using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Domain entities
    public DbSet<Household> Households => Set<Household>();
    public DbSet<UserHousehold> UserHouseholds => Set<UserHousehold>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<PlannedMeal> PlannedMeals => Set<PlannedMeal>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<Pantry> Pantries => Set<Pantry>();
    public DbSet<PantryItem> PantryItems => Set<PantryItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // UserHousehold - Composite key
        builder.Entity<UserHousehold>()
            .HasKey(uh => new { uh.UserId, uh.HouseholdId });

        builder.Entity<UserHousehold>()
            .HasOne(uh => uh.Household)
            .WithMany(h => h.UserHouseholds)
            .HasForeignKey(uh => uh.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        // RecipeIngredient - Composite key
        builder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        builder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Recipe - Relationships
        builder.Entity<Recipe>()
            .HasOne(r => r.Household)
            .WithMany(h => h.Recipes)
            .HasForeignKey(r => r.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Recipe>()
            .HasIndex(r => r.Name);

        // MealPlan - Relationships
        builder.Entity<MealPlan>()
            .HasOne(mp => mp.Household)
            .WithMany(h => h.MealPlans)
            .HasForeignKey(mp => mp.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MealPlan>()
            .HasIndex(mp => new { mp.HouseholdId, mp.WeekStartDate })
            .IsUnique();

        // PlannedMeal - Relationships
        builder.Entity<PlannedMeal>()
            .HasOne(pm => pm.MealPlan)
            .WithMany(mp => mp.PlannedMeals)
            .HasForeignKey(pm => pm.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PlannedMeal>()
            .HasOne(pm => pm.Recipe)
            .WithMany(r => r.PlannedMeals)
            .HasForeignKey(pm => pm.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShoppingList - Relationships
        builder.Entity<ShoppingList>()
            .HasOne(sl => sl.Household)
            .WithMany()
            .HasForeignKey(sl => sl.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ShoppingList>()
            .HasOne(sl => sl.MealPlan)
            .WithMany()
            .HasForeignKey(sl => sl.MealPlanId)
            .OnDelete(DeleteBehavior.SetNull);

        // ShoppingListItem - Relationships
        builder.Entity<ShoppingListItem>()
            .HasOne(sli => sli.ShoppingList)
            .WithMany(sl => sl.Items)
            .HasForeignKey(sli => sli.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ShoppingListItem>()
            .HasOne(sli => sli.Ingredient)
            .WithMany()
            .HasForeignKey(sli => sli.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Pantry - Relationships
        builder.Entity<Pantry>()
            .HasOne(p => p.Household)
            .WithMany()
            .HasForeignKey(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Pantry>()
            .HasIndex(p => p.HouseholdId)
            .IsUnique();

        // PantryItem - Relationships
        builder.Entity<PantryItem>()
            .HasOne(pi => pi.Pantry)
            .WithMany(p => p.Items)
            .HasForeignKey(pi => pi.PantryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PantryItem>()
            .HasOne(pi => pi.Ingredient)
            .WithMany()
            .HasForeignKey(pi => pi.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ingredient - Unique name and index
        builder.Entity<Ingredient>()
            .HasIndex(i => i.Name)
            .IsUnique();

        // Seed common ingredients
        SeedIngredients(builder);
    }

    private void SeedIngredients(ModelBuilder builder)
    {
        builder.Entity<Ingredient>().HasData(
            // Proteins
            new Ingredient { Id = 1, Name = "Chicken Breast", Category = "Meat" },
            new Ingredient { Id = 2, Name = "Ground Beef", Category = "Meat" },
            new Ingredient { Id = 3, Name = "Salmon Fillet", Category = "Fish" },
            new Ingredient { Id = 4, Name = "Eggs", Category = "Dairy" },

            // Carbs
            new Ingredient { Id = 10, Name = "Pasta", Category = "Grains" },
            new Ingredient { Id = 11, Name = "Rice", Category = "Grains" },
            new Ingredient { Id = 12, Name = "Bread", Category = "Grains" },
            new Ingredient { Id = 13, Name = "Potatoes", Category = "Vegetables" },

            // Vegetables
            new Ingredient { Id = 20, Name = "Onion", Category = "Vegetables" },
            new Ingredient { Id = 21, Name = "Garlic", Category = "Vegetables" },
            new Ingredient { Id = 22, Name = "Tomatoes", Category = "Vegetables" },
            new Ingredient { Id = 23, Name = "Bell Pepper", Category = "Vegetables" },
            new Ingredient { Id = 24, Name = "Carrots", Category = "Vegetables" },
            new Ingredient { Id = 25, Name = "Broccoli", Category = "Vegetables" },

            // Dairy
            new Ingredient { Id = 30, Name = "Milk", Category = "Dairy" },
            new Ingredient { Id = 31, Name = "Cheese", Category = "Dairy" },
            new Ingredient { Id = 32, Name = "Butter", Category = "Dairy" },
            new Ingredient { Id = 33, Name = "Cream", Category = "Dairy" },

            // Pantry staples
            new Ingredient { Id = 40, Name = "Olive Oil", Category = "Oils" },
            new Ingredient { Id = 41, Name = "Salt", Category = "Seasonings" },
            new Ingredient { Id = 42, Name = "Black Pepper", Category = "Seasonings" },
            new Ingredient { Id = 43, Name = "Flour", Category = "Baking" },
            new Ingredient { Id = 44, Name = "Sugar", Category = "Baking" }
        );
    }
}
