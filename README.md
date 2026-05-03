# RentalApp - Library of Things Peer-to-Peer Rental Marketplace

[![codecov](https://codecov.io/gh/40591106/StarterApp/branch/main/graph/badge.svg)](https://codecov.io/gh/40591106/StarterApp)

A .NET MAUI mobile application that allows community members to list items for rent, discover nearby items using location-based search, manage rental requests, and leave reviews.

## Features

### Tier 1 - Core Features
- User authentication (local and API-based)
- Item listing with title, description, daily rate, category and location
- Browse and search items by category or keyword
- View detailed item information including reviews and ratings
- Submit rental requests

### Tier 2 - Intermediate Features
- Location-based item discovery using GPS (find items within configurable radius)
- Full rental workflow (Requested → Approved/Rejected → Out for Rent → Overdue → Returned → Completed)
- Double-booking prevention
- Approve/reject rental requests (owner side)
- Submit and view reviews after completed rentals
- Average rating display on item detail and user profile

### Tier 3 - Advanced Features
- Automatic overdue detection (Out for Rent → Overdue state transition)
- Local device notifications for overdue rentals

## Architecture

The project follows a clean architecture with three projects:
RentalApp/
├── RentalApp/                  # Main MAUI project (Views, ViewModels, Services, Repositories)
├── RentalApp.Database/         # Shared library (Models, Repositories, DbContext)
├── RentalApp.Migrations/       # EF Core migrations
└── RentalApp.Test/             # xUnit test project
### Design Patterns
- **MVVM** — Views bind to ViewModels via data binding, no logic in code-behind
- **Repository Pattern** — `IRepository<T>` base interface with local and API implementations
- **Service Layer** — Business logic in `RentalService`, `ReviewService`, `LocationService`

### Authentication Modes
The app supports two modes controlled by `useSharedApi` in `MauiProgram.cs`:
- `true` (default) — uses the shared SET09102 API for all data
- `false` — uses local PostgreSQL database via Entity Framework Core

## Compatibility

| Name | Version |
|---|---|
| .NET | 10.0 |
| .NET MAUI | 10.0 |
| PostgreSQL | 16 |
| xUnit | 2.x |

## Prerequisites

1. **.NET SDK 10.0** installed
2. **Docker Desktop** installed and running
3. **Android Emulator** running with ADB server on host
4. **VS Code** with C# Dev Kit extension

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/40591106/StarterApp.git
cd RentalApp
```

### 2. Configure the database connection

Copy the template and update with your credentials:

```bash
cp RentalApp.Database/appsettings.json.template RentalApp.Database/appsettings.json
```

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DevelopmentConnection": "Host=10.0.2.2:5432;Username=app_user;Password=app_password;Database=appdb"
  }
}
```

### 3. Start the Docker environment

```bash
docker compose down -v   # Clean slate
docker compose up -d     # Start containers
```

### 4. Open in VS Code Dev Container

Open the project folder in VS Code and click **Reopen in Container** when prompted.

### 5. Build the project

```bash
dotnet clean
dotnet build -c Debug
```

### 6. Install and run on emulator

```bash
adb uninstall com.companyname.rentalapp
adb install -r bin/Debug/net10.0-android/com.companyname.rentalapp-Signed.apk
```

## Running Tests

```bash
dotnet test RentalApp.Test/RentalApp.Test.csproj
```

To generate a coverage report:

```bash
dotnet test RentalApp.Test/RentalApp.Test.csproj --collect:"XPlat Code Coverage"
reportgenerator \
  -reports:"RentalApp.Test/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html \
  "-assemblyfilters:+RentalApp.Database" \
  "-classfilters:-RentalApp.Database.Migrations.*"
```

Open `coverage-report/index.html` to view the full report.

## API

This app connects to the SET09102 API at:
`https://set09102-api.b-davison.workers.dev`

For full API documentation see the Swagger UI at the above URL.

## CI/CD

GitHub Actions workflow runs on every push to `main` and on pull requests:
- Builds `RentalApp.Database`, `RentalApp.Migrations` and `RentalApp.Test`
- Runs all 100 unit tests
- Uploads test results as a build artifact

## Testing

The test suite covers:
- **Repositories** — `ItemRepository`, `RentalRepository`, `ReviewRepository`
- **Services** — `RentalService`, `ReviewService`, `LocationService`
- **ViewModels** — `ItemsListViewModel`, `RentalsViewModel`, `NearbyItemsViewModel`, `ReviewsViewModel`, `ItemDetailViewModel`
- **Mocking** — `MockLocationService` for GPS abstraction

**Coverage:** 70.1% on RentalApp.Database, 67% full solution (via Codecov)