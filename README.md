# DonationHub

DonationHub is a donation management platform built with **ASP.NET Core Web API** and **Clean Architecture**. It provides authentication, role-based authorization, and category management as part of Sprint 1.

## Overview

The project is structured using Clean Architecture to separate business rules, application logic, and infrastructure concerns.

## Prerequisites

Make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server
- Visual Studio 2022 or a compatible IDE
- Entity Framework Core CLI tools

Install the EF Core CLI tool if you do not already have it:

```bash
dotnet tool install --global dotnet-ef
```

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd DonationHub
```

### 2. Configure the Database Connection

The application uses SQL Server.

Update the `DefaultConnection` value in `DonationHub.Api/appsettings.json` to match your local SQL Server configuration.

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=DonationHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Configure User Secrets

The API project uses .NET User Secrets to keep sensitive development settings outside the source code.

The project already has a `UserSecretsId` configured, so you do not need to initialize User Secrets again.

From the solution root, configure the following values:

```bash
dotnet user-secrets set "Jwt:SecretKey" "<your-jwt-secret>" --project DonationHub.Api/DonationHub.Api.csproj

dotnet user-secrets set "SeedAdmin:Email" "<admin-email>" --project DonationHub.Api/DonationHub.Api.csproj
dotnet user-secrets set "SeedAdmin:Password" "<admin-password>" --project DonationHub.Api/DonationHub.Api.csproj
dotnet user-secrets set "SeedAdmin:FullName" "<admin-full-name>" --project DonationHub.Api/DonationHub.Api.csproj
```

For development testing, configure the Campaign Manager and Donor accounts:

```bash
dotnet user-secrets set "DevelopmentSeedUsers:CampaignManager:Email" "<campaign-manager-email>" --project DonationHub.Api/DonationHub.Api.csproj
dotnet user-secrets set "DevelopmentSeedUsers:CampaignManager:Password" "<campaign-manager-password>" --project DonationHub.Api/DonationHub.Api.csproj
dotnet user-secrets set "DevelopmentSeedUsers:CampaignManager:FullName" "<campaign-manager-full-name>" --project DonationHub.Api/DonationHub.Api.csproj

dotnet user-secrets set "DevelopmentSeedUsers:Donor:Email" "<donor-email>" --project DonationHub.Api/DonationHub.Api.csproj
dotnet user-secrets set "DevelopmentSeedUsers:Donor:Password" "<donor-password>" --project DonationHub.Api/DonationHub.Api.csproj
dotnet user-secrets set "DevelopmentSeedUsers:Donor:FullName" "<donor-full-name>" --project DonationHub.Api/DonationHub.Api.csproj
```

Use a JWT secret key containing at least 32 characters.

**Important:**
- Development seed users are created only when the application runs in the Development environment.
- Never commit passwords, JWT secrets, or other sensitive configuration values to source control.
- The seeded Admin account is assigned the Admin role.
- Development seed accounts are assigned their corresponding roles.

## Database Migrations

The project uses Entity Framework Core with SQL Server.

To apply the existing migrations and create or update the database, run this command from the solution root:

```bash
dotnet ef database update --project DonationHub.Infrastructure/DonationHub.Infrastructure.csproj --startup-project DonationHub.Api/DonationHub.Api.csproj
```

If you need to create a new migration:

```bash
dotnet ef migrations add <MigrationName> --project DonationHub.Infrastructure/DonationHub.Infrastructure.csproj --startup-project DonationHub.Api/DonationHub.Api.csproj
```

## Running the Application

Run the API project:

```bash
dotnet run --project DonationHub.Api/DonationHub.Api.csproj
```

When running in the Development environment, Swagger UI is available at:

```text
https://localhost:<port>/swagger
```

Replace `<port>` with the HTTPS port displayed in the application output.

## API Documentation

The API uses Swagger/OpenAPI for endpoint documentation.

Swagger includes Bearer authentication support. To test protected endpoints:

1. Register or log in to obtain an access token.
2. Copy the returned JWT access token.
3. Click **Authorize** in Swagger UI.
4. Enter the token using the Bearer authentication scheme.
5. Test the protected endpoints.

## Main API Endpoints

All endpoints are grouped under the `/api/v1` route prefix.

### Authentication

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/v1/auth/register` | Anonymous |
| POST | `/api/v1/auth/login` | Anonymous |
| POST | `/api/v1/auth/refresh` | Anonymous |
| POST | `/api/v1/auth/logout` | Authenticated |
| GET | `/api/v1/auth/me` | Authenticated |
| POST | `/api/v1/admin/staff` | Admin |

### Categories

| Method | Endpoint | Access |
|---|---|---|
| GET | `/api/v1/categories` | Anonymous |
| GET | `/api/v1/categories/{id}` | Anonymous |
| POST | `/api/v1/categories` | Admin |
| PUT | `/api/v1/categories/{id}` | Admin |
| DELETE | `/api/v1/categories/{id}` | Admin |

## Authentication and Authorization

The project uses ASP.NET Core Identity and JWT Bearer authentication.

- Users can register and log in using their email and password.
- Passwords must contain at least 8 characters, including uppercase, lowercase, and numeric characters.
- Email addresses must be unique.
- JWT access tokens have a configurable expiration time.
- Refresh tokens have a configurable expiration period and are stored as hashes.
- Refresh token rotation is supported.
- Role-based authorization is used to protect administrative operations.

### User Roles

- **Admin:** Can manage categories and create staff accounts.
- **CampaignManager:** Used for campaign management access.
- **Donor:** Default role assigned to newly registered users.

## Default Data Seeding

On application startup:

- Required identity roles are seeded.
- The configured Admin account is created if it does not already exist.
- Development test accounts are seeded in the Development environment.
- Default bilingual categories are added if they do not already exist.

## Project Structure

The solution follows Clean Architecture and is divided into the following projects:

```text
DonationHub/
│
├── DonationHub.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Common/
│   └── ValueObjects/
│
├── DonationHub.Application/
│   ├── Abstractions/
│   │   ├── Repositories/
│   │   └── Identity/
│   │
│   ├── Features/
│   │   ├── Authentication/
│   │   │   ├── Register/
│   │   │   ├── Login/
│   │   │   ├── RefreshToken/
│   │   │   ├── Logout/
│   │   │   ├── GetCurrentUser/
│   │   │   └── CreateStaff/
│   │   │
│   │   └── Categories/
│   │       ├── CreateCategory/
│   │       ├── GetAllCategories/
│   │       ├── GetCategoryById/
│   │       ├── UpdateCategory/
│   │       ├── DeleteCategory/
│   │       ├── SetCategoryDescription/
│   │       └── RemoveCategoryDescription/
│   │
│   └── DependencyInjection.cs
│
├── DonationHub.Infrastructure/
│   ├── Identity/
│   ├── Options/
│   ├── Persistence/
│   ├── Repositories/
│   ├── Migrations/
│   └── DependencyInjection.cs
│
├── DonationHub.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── CategoryController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── .gitignore
├── DonationHub.sln
└── README.md
```

## Design Decisions

### Clean Architecture

The solution separates business rules, application use cases, and external concerns into independent layers. Dependencies point inward, keeping the Domain independent of infrastructure details.

### Feature-Based Application Structure

Application functionality is organized by feature to keep related use cases and their components together.

### Dependency Injection

Each layer exposes its own dependency registration through separate extension methods. The API project composes these registrations during application startup.

### Repository and Unit of Work

Write operations use the Generic Repository and Unit of Work patterns to organize data changes and persistence.

### Configuration and Secrets

Application settings are separated from sensitive development configuration. .NET User Secrets are used for local development credentials and JWT secrets.

### Authentication

ASP.NET Core Identity handles user and role management, while JWT Bearer authentication protects API endpoints. Refresh tokens are stored as hashes and rotated when refreshed.

## Sprint 1 Contents

- Clean Architecture solution structure.
- Application and Infrastructure dependency injection.
- SQL Server and Entity Framework Core integration.
- Identity roles and user management.
- User registration and login.
- JWT access token generation and validation.
- Refresh token management and rotation.
- Logout and current-user endpoints.
- Admin staff account creation.
- Category management endpoints.
- Role-based authorization.
- Swagger/OpenAPI documentation with Bearer authentication.
- Default roles, users, and categories seeding.

---
