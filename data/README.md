# Data Directory

This directory contains local development databases.

## Files

- `mealplanner.db` - SQLite database for local development
- `*.db-shm`, `*.db-wal` - SQLite journal files

## Important

⚠️ **This directory is git-ignored** - database files are not committed to version control.

## Production

In production (Railway.dev), the app uses PostgreSQL instead of SQLite. Connection strings are configured via environment variables.

## Resetting the Database

To reset your local database:

```bash
# Delete the database
rm data/mealplanner.db*

# Recreate from migrations
dotnet ef database update --project src/MealPlanner.Infrastructure --startup-project src/MealPlanner.Web
```
