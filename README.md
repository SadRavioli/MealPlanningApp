# Meal Planning Application

A mobile-first meal planning application built with ASP.NET Core using Clean Architecture.

## Project Structure

```
MealPlanner.sln
├── src/
│   ├── MealPlanner.Domain/         # Core business entities and rules
│   ├── MealPlanner.Application/    # Business logic and use cases
│   ├── MealPlanner.Infrastructure/ # Data access and external services
│   └── MealPlanner.Web/            # ASP.NET Core web application
└── tests/
    ├── MealPlanner.Domain.Tests/
    ├── MealPlanner.Application.Tests/
    └── MealPlanner.Infrastructure.Tests/
```

## Tech Stack

- .NET 9.0
- ASP.NET Core + Razor Pages
- Entity Framework Core
- SQLite (local development) / PostgreSQL (production)
- ASP.NET Core Identity
- Tailwind CSS
- HTMX
- Alpine.js

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Running Locally

1. Clone the repository
2. Run the application:
   ```bash
   dotnet run --project src/MealPlanner.Web
   ```

### Running Tests

```bash
dotnet test
```

## Database

- **Local Development**: Uses SQLite (no setup required)
- **Production**: PostgreSQL on Railway.dev

## Features

### Phase 1 (Current)
- User authentication and registration
- Recipe management (CRUD)
- Weekly meal planning
- Mobile-first responsive design

### Future Phases
- Smart meal rotation and suggestions
- Shopping list generation
- Pantry tracking
- Supermarket integration with pricing
- PWA offline support
