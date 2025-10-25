# Meal Planning Application - Implementation Plan

## Project Overview
A mobile-first meal planning application built with ASP.NET Core MVC using Clean Architecture. Solves the weekly "what's for dinner" problem with intelligent meal rotation, shopping list generation, and eventual supermarket integration.

## Tech Stack
- **.NET 8** (LTS)
- **ASP.NET Core MVC** with Razor views
- **Entity Framework Core** with PostgreSQL
- **ASP.NET Core Identity** for authentication
- **Tailwind CSS** for mobile-first responsive design
- **HTMX** for dynamic server interactions
- **Alpine.js** for lightweight client-side interactivity
- **PWA** features (service worker, manifest)
- **Railway.dev** for hosting (PostgreSQL included)

## Architecture
Clean Architecture with four layers:

### 1. Domain Layer
Core business entities and interfaces. No dependencies on other layers.

**Entities:**
- `User` - Application user
- `Household` - Shared meal planning unit
- `UserHousehold` - Join table with roles
- `Recipe` - Meal recipes with serving size
- `Ingredient` - Generic ingredients (e.g., "chicken breast")
- `RecipeIngredient` - Join table with quantity, unit, ingredient_id, recipe_id
- `MealPlan` - Weekly plan for a household
- `PlannedMeal` - Specific meal on specific day/meal slot
- `ShoppingList` - Generated from meal plans
- `ShoppingListItem` - Individual items on shopping list
- `Pantry` - Household inventory
- `PantryItem` - Items currently in pantry
- `Product` - Actual supermarket products (Phase 4)
- `ProductMapping` - Links ingredients to products with quality tier (Phase 4)
- `MealPreference` - Household preferences for meal frequency, dietary restrictions

**Enums:**
- `MealType` (Breakfast, Lunch, Dinner)
- `DayOfWeek` (Monday-Sunday)
- `MeasurementUnit` (g, kg, ml, l, cups, tbsp, tsp, pieces, etc.)
- `QualityTier` (Standard, Organic, FreeRange, Premium) - Phase 4
- `HouseholdRole` (Admin, Member)

**Value Objects:**
- `Quantity` (decimal Amount, MeasurementUnit Unit)

### 2. Application Layer
Business logic, use cases, service interfaces, DTOs.

**Services/Use Cases (Phase 1):**
- `IRecipeService` - CRUD operations, recipe scaling
- `IMealPlanService` - Create/update weekly plans, add meals to days
- `IUserService` - User registration, profile management
- `IHouseholdService` - Create household (auto-create on signup initially)

**DTOs (Phase 1):**
- `RecipeDto`, `CreateRecipeDto`, `UpdateRecipeDto`
- `RecipeIngredientDto`
- `MealPlanDto`, `PlannedMealDto`
- `UserDto`, `RegisterDto`

**Validators:**
- Use FluentValidation for DTO validation

### 3. Infrastructure Layer
Data persistence, external services, repository implementations.

**Repositories:**
- `IRecipeRepository`
- `IMealPlanRepository`
- `IIngredientRepository`
- `IHouseholdRepository`
- Generic `IRepository<T>` base

**DbContext:**
- `ApplicationDbContext` using EF Core
- Configure all entities and relationships
- Seed initial data (common ingredients, measurement units)

**Identity:**
- Extend `IdentityUser` with household relationship
- Configure ASP.NET Core Identity

### 4. Web/Presentation Layer
Controllers, Razor views, API endpoints, frontend assets.

**Controllers (Phase 1):**
- `HomeController` - Landing, dashboard
- `RecipeController` - Recipe CRUD
- `MealPlanController` - Weekly meal planning interface
- `AccountController` - Login, register, logout

**Views Structure:**
```
Views/
├── Shared/
│   ├── _Layout.cshtml (mobile-first navigation)
│   ├── _LoginPartial.cshtml
│   └── Components/
├── Home/
│   └── Index.cshtml (dashboard/overview)
├── Recipe/
│   ├── Index.cshtml (recipe list)
│   ├── Details.cshtml
│   ├── Create.cshtml
│   └── Edit.cshtml
├── MealPlan/
│   ├── Index.cshtml (weekly planner view)
│   └── _MealCard.cshtml (partial for HTMX)
└── Account/
    ├── Login.cshtml
    └── Register.cshtml
```

**API Endpoints (for HTMX):**
- `POST /api/mealplan/add-meal` - Add meal to specific day
- `DELETE /api/mealplan/remove-meal/{id}` - Remove planned meal
- `GET /api/mealplan/meal-card/{id}` - Return meal card HTML partial
- `POST /api/recipe/scale` - Scale recipe servings

## Phase 1: Foundation (Start Here)

### Step 1: Project Setup
1. Create solution with Clean Architecture structure:
   ```
   MealPlanner.sln
   ├── src/
   │   ├── MealPlanner.Domain/
   │   ├── MealPlanner.Application/
   │   ├── MealPlanner.Infrastructure/
   │   └── MealPlanner.Web/
   └── tests/
       ├── MealPlanner.Domain.Tests/
       ├── MealPlanner.Application.Tests/
       └── MealPlanner.Infrastructure.Tests/
   ```

2. Add project references:
   - Web → Application, Infrastructure
   - Infrastructure → Application, Domain
   - Application → Domain

3. Install NuGet packages:
   - **Domain**: none (keep pure)
   - **Application**: FluentValidation, FluentValidation.DependencyInjectionExtensions
   - **Infrastructure**: 
     - Microsoft.EntityFrameworkCore
     - Npgsql.EntityFrameworkCore.PostgreSQL
     - Microsoft.AspNetCore.Identity.EntityFrameworkCore
   - **Web**:
     - All Infrastructure packages (transitively)
     - Microsoft.AspNetCore.Identity.UI (optional for scaffolded pages)

### Step 2: Domain Layer Implementation
1. Create all entities in `Domain/Entities/`
2. Create enums in `Domain/Enums/`
3. Create value objects in `Domain/ValueObjects/`
4. Create repository interfaces in `Domain/Repositories/`
5. Keep entities focused on business rules and invariants

**Key Domain Rules:**
- Recipe must have at least one ingredient
- ServingSize must be positive
- PlannedMeal must belong to a valid MealPlan
- Recipe scaling multiplies all ingredient quantities

### Step 3: Application Layer Implementation
1. Create DTOs in `Application/DTOs/`
2. Create service interfaces in `Application/Services/`
3. Implement services in `Application/Services/Implementations/`
4. Add FluentValidation validators in `Application/Validators/`
5. Configure dependency injection extensions

**Recipe Scaling Logic:**
```csharp
public RecipeDto ScaleRecipe(int recipeId, int newServings)
{
    var recipe = _repository.GetById(recipeId);
    var scaleFactor = (decimal)newServings / recipe.ServingSize;
    
    var scaledIngredients = recipe.Ingredients.Select(ri => 
        new RecipeIngredientDto 
        {
            IngredientName = ri.Ingredient.Name,
            Quantity = ri.Quantity * scaleFactor,
            Unit = ri.Unit
        }).ToList();
    
    return new RecipeDto { /* map properties */ };
}
```

### Step 4: Infrastructure Layer Implementation

1. **Create ApplicationDbContext:**
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<MealPlan> MealPlans { get; set; }
    public DbSet<PlannedMeal> PlannedMeals { get; set; }
    public DbSet<Household> Households { get; set; }
    public DbSet<UserHousehold> UserHouseholds { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure relationships and constraints
        builder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            
        builder.Entity<UserHousehold>()
            .HasKey(uh => new { uh.UserId, uh.HouseholdId });
            
        // Add indexes for performance
        builder.Entity<Recipe>()
            .HasIndex(r => r.Name);
            
        // Seed common ingredients
        builder.Entity<Ingredient>().HasData(
            new Ingredient { Id = 1, Name = "Chicken Breast" },
            new Ingredient { Id = 2, Name = "Pasta" },
            // Add more common ingredients
        );
    }
}
```

2. **Implement repositories:**
   - Generic repository base class
   - Specific repository implementations with custom queries

3. **Configure EF Core migrations:**
   - Initial migration with all Phase 1 entities
   - Seed data migration

4. **Configure Identity:**
   - Extend IdentityUser as ApplicationUser with HouseholdId
   - Configure password requirements
   - Set up cookie authentication

### Step 5: Web Layer Implementation

1. **Configure Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register application services
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IMealPlanService, MealPlanService>();
// etc.

var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

2. **Create mobile-first layout (_Layout.cshtml):**
   - Bottom navigation bar (mobile)
   - Responsive top navigation (desktop)
   - Tailwind CSS setup
   - HTMX and Alpine.js script includes

3. **Implement controllers with API endpoints:**
   - Return views for standard requests
   - Return HTML partials for HTMX requests

4. **Create Razor views:**
   - Mobile-first design principles
   - Touch-friendly UI elements (min 44px tap targets)
   - Use HTMX attributes for dynamic interactions

### Step 6: Frontend Setup

1. **Install Tailwind CSS:**
   - Via npm or CDN (CDN simpler for starting)
   - Configure for mobile-first breakpoints
   - Create custom colours for app branding

2. **Add HTMX:**
   - Include via CDN: `<script src="https://unpkg.com/htmx.org@1.9.10"></script>`
   - Use `hx-get`, `hx-post`, `hx-target`, `hx-swap` attributes

3. **Add Alpine.js:**
   - Include via CDN: `<script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>`
   - Use for dropdowns, modals, form interactions

4. **Create key UI components:**
   - Recipe card component
   - Meal planner grid (7 days × 3 meals)
   - Mobile-friendly forms
   - Bottom sheet/modal for adding meals

### Step 7: Core Features Implementation

**Recipe Management:**
- List all recipes (with search/filter)
- Create new recipe with ingredients
- Edit existing recipe
- Delete recipe (with confirmation)
- View recipe details with scaled ingredients

**Meal Planning:**
- Weekly view (current week by default)
- Add meal to specific day/meal slot (drag or tap)
- Remove meal from plan
- Navigate to previous/next week
- Empty state prompts

**User Dashboard:**
- Show current week's meal plan
- Quick stats (recipes count, this week's meals)
- Quick actions (add recipe, plan week)

### Step 8: Railway.dev Deployment

1. **Prepare for deployment:**
   - Add Railway.json configuration
   - Set up environment variables
   - Configure connection string from Railway PostgreSQL

2. **Database setup:**
   - Run migrations on Railway PostgreSQL
   - Seed initial data

3. **Deploy:**
   - Connect GitHub repository to Railway
   - Configure build settings
   - Deploy and test

## Database Schema (Phase 1)

```sql
-- Users (via Identity)
AspNetUsers (
    Id, UserName, Email, PasswordHash, HouseholdId (FK)
)

-- Households
Households (
    Id PK,
    Name VARCHAR(200),
    CreatedAt DATETIME
)

-- User-Household mapping
UserHouseholds (
    UserId FK,
    HouseholdId FK,
    Role INT, -- Enum: Admin=1, Member=2
    JoinedAt DATETIME,
    PRIMARY KEY (UserId, HouseholdId)
)

-- Ingredients
Ingredients (
    Id PK,
    Name VARCHAR(200) UNIQUE,
    Category VARCHAR(100) -- e.g., "Meat", "Vegetables", "Dairy"
)

-- Recipes
Recipes (
    Id PK,
    Name VARCHAR(200),
    Description TEXT,
    Instructions TEXT,
    PrepTimeMinutes INT,
    CookTimeMinutes INT,
    ServingSize INT,
    HouseholdId FK, -- Owner of recipe
    CreatedAt DATETIME,
    UpdatedAt DATETIME
)

-- Recipe Ingredients (join table)
RecipeIngredients (
    RecipeId FK,
    IngredientId FK,
    Quantity DECIMAL(10,2),
    Unit INT, -- Enum
    Notes VARCHAR(500), -- e.g., "finely chopped"
    PRIMARY KEY (RecipeId, IngredientId)
)

-- Meal Plans
MealPlans (
    Id PK,
    HouseholdId FK,
    WeekStartDate DATE, -- Monday of the week
    CreatedAt DATETIME
)

-- Planned Meals
PlannedMeals (
    Id PK,
    MealPlanId FK,
    RecipeId FK,
    DayOfWeek INT, -- Enum: 0=Monday, 6=Sunday
    MealType INT, -- Enum: Breakfast, Lunch, Dinner
    ServingsPlanned INT, -- For scaling
    Notes VARCHAR(500)
)
```

## Phase 2: Intelligence (Future)

**Features to implement:**
- Meal rotation tracking table (tracks when each recipe was last used)
- Preference system (frequency per recipe, dietary restrictions, cuisine preferences)
- Smart meal suggestion algorithm
- "Generate Week" functionality with constraints
- Recipe rating/favorites

**New entities:**
- `MealHistory` - Track when recipes were used
- `RecipePreference` - User/household preferences for specific recipes
- `DietaryRestriction` - Household dietary requirements

## Phase 3: Shopping & Pantry (Future)

**Features:**
- Pantry inventory tracking
- Shopping list generation from meal plan
- Manual shopping list management
- Mark items as "already have"
- Export/copy shopping list

**New entities:**
- `Pantry` (already in Phase 1 schema)
- `PantryItem` (ingredient, quantity, expiry date)
- `ShoppingList` (already in Phase 1 schema)
- `ShoppingListItem` (ingredient, quantity, checked status)

## Phase 4: Supermarket Integration (Future)

**Features:**
- Product catalogue (scraped or API)
- Ingredient → Product mapping
- Quality tiers (standard, organic, free-range)
- Price tracking and comparison
- Dynamic product selection based on prices/preferences

**New entities:**
- `Product` (already in Phase 1 schema)
- `ProductMapping` (already in Phase 1 schema)
- `PriceHistory` - Track price changes over time
- `ProductPreference` - User preferences for product quality/brand

**Integration approach:**
- Start with web scraping for Tesco/Asda
- Build product matching algorithm (fuzzy matching)
- Price update scheduled job
- Fallback to manual entry if no match found

## Phase 5: Polish & Advanced Features (Future)

**Features:**
- PWA offline support
- Household member invitations
- Recipe import from URLs (parse recipe websites)
- Nutritional information
- Recipe photos
- Meal prep mode (batch cooking)
- Leftover tracking
- Calendar integration

## Mobile-First UI Guidelines

**Key Principles:**
1. **Touch targets:** Minimum 44×44px for all interactive elements
2. **Bottom navigation:** Primary actions at thumb-reach
3. **Vertical scrolling:** Primary interaction direction
4. **Minimal text input:** Use selectors and pickers where possible
5. **Immediate feedback:** Loading states, optimistic UI updates
6. **Gestures:** Swipe to delete, pull to refresh

**Screen Priorities:**
1. **Home/Dashboard** - Current week overview, quick actions
2. **Meal Planner** - Weekly grid, add meals
3. **Recipes** - List with search, quick view
4. **Shopping List** - Simple checklist (Phase 3)

**Navigation Structure:**
```
Bottom Nav (Mobile):
├── Home (dashboard icon)
├── Plan (calendar icon)
├── Recipes (book icon)
└── More (menu icon)
    ├── Shopping List
    ├── Pantry
    ├── Settings
    └── Account
```

## Testing Strategy

**Unit Tests:**
- Domain entities business logic
- Application services
- Recipe scaling calculations
- Meal rotation algorithm (Phase 2)

**Integration Tests:**
- Repository operations
- Database queries
- Identity integration

**End-to-End Tests (optional):**
- Playwright or Selenium
- Key user journeys (create recipe, plan week)

## Development Workflow

1. **Start with Domain:** Get entities and business rules right
2. **Application layer:** Implement use cases with clean interfaces
3. **Infrastructure:** Wire up persistence and data access
4. **Web layer:** Build UI and controllers
5. **Iterate:** Add features incrementally, test thoroughly

## Success Criteria - Phase 1

- [ ] User can register and log in
- [ ] User can create, edit, delete recipes with ingredients
- [ ] User can scale recipes to different serving sizes
- [ ] User can view a weekly meal plan grid
- [ ] User can add recipes to specific days/meals
- [ ] User can remove meals from the plan
- [ ] UI is fully responsive and mobile-friendly
- [ ] App is deployed to Railway.dev and accessible online

## Notes for Claude Code

- Follow Clean Architecture strictly - respect layer boundaries
- Use async/await throughout for database operations
- Implement proper error handling and validation
- Keep controllers thin - business logic belongs in services
- Use DTOs to avoid exposing domain entities directly
- Write descriptive commit messages
- Test each feature as you build it
- Mobile-first CSS - start with mobile styles, add desktop breakpoints
- HTMX for any dynamic page updates - avoid full page reloads
- Keep JavaScript minimal - use Alpine.js for simple interactions only

## Useful Commands

```bash
# Create migration
dotnet ef migrations add InitialCreate --project src/MealPlanner.Infrastructure --startup-project src/MealPlanner.Web

# Update database
dotnet ef database update --project src/MealPlanner.Infrastructure --startup-project src/MealPlanner.Web

# Run application
dotnet run --project src/MealPlanner.Web

# Run tests
dotnet test
```

## Connection String (Railway)

Railway provides PostgreSQL connection string via environment variable:
```
DATABASE_URL=postgresql://username:password@host:port/database
```

Parse this in Program.cs and configure Npgsql accordingly.

---

**Ready to start? Begin with Step 1: Project Setup**