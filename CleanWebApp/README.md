# Example Template - Clean Architecture Web API

This is a production-ready ASP.NET Core Web API template using the **Controller pattern** and a four-project Clean Architecture layout.

## Features

- **Clean Architecture Layout**: Domain, Application, Infrastructure, and WebApp projects with inward dependencies
- **Controller-Based API**: Traditional MVC-style controllers with attribute routing
- **Entity Framework Core**: Database abstraction with support for SQLite (dev) and PostgreSQL (production)
- **Authentication & Authorization**: JWT-based authentication with customizable policies
- **Health Checks**: Comprehensive health check endpoints for monitoring
- **Rate Limiting**: Built-in rate limiting with IP-based partitioning
- **API Key Middleware**: Optional API key authentication
- **Correlation ID Middleware**: Request tracking and logging
- **Error Handling**: Global exception handling with detailed error responses
- **Swagger/OpenAPI**: Interactive API documentation
- **Validation**: Custom validation attributes for robust data validation
- **CORS Support**: Configurable CORS policies

## Project Structure

```
ExampleTemplate.Domain/
├── Constants/         # Domain constants
├── Models/Options/   # Strongly typed configuration models
└── Validation/        # Validation attributes and rules

ExampleTemplate.Application/
├── Extensions/        # Application service registration
└── Interfaces/        # Service interfaces and application ports

ExampleTemplate.Infrastructure/
├── Database/          # EF Core contexts and provider implementations
│   └── EntityFramework/
│       └── PostgreSql/
├── Extensions/        # Infrastructure dependency registration
└── HealthChecks/      # Infrastructure health checks

ExampleTemplate.WebApp/
├── Controllers/       # API controllers
├── Middleware/        # HTTP middleware
├── Policies/          # Authorization policies
└── Properties/        # Launch settings
```

## Dependency Direction

Dependencies flow inward only:

```text
ExampleTemplate.WebApp -> ExampleTemplate.Application -> ExampleTemplate.Domain
ExampleTemplate.Infrastructure -> ExampleTemplate.Application -> ExampleTemplate.Domain
ExampleTemplate.WebApp -> ExampleTemplate.Infrastructure
```

The WebApp project is the composition root. It registers Application services and Infrastructure implementations in `Program.cs`.

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- PostgreSQL (optional, for production database)

### Configuration

Update `appsettings.json` or use user secrets:

```json
{
  "Database": {
    "Provider": "sqlite",  // or "postgresql"
    "ConnectionString": "" // Required for PostgreSQL
  },
  "RateLimit": {
    "Enabled": true,
    "PermitLimit": 100,
    "WindowSeconds": 60,
    "QueueLimit": 0
  },
  "ApiKey": "your-api-key-here",
  "Authentication": {
    "WellKnownEndpoint": "https://your-identity-provider/.well-known/openid-configuration",
    "Issuer": "https://your-identity-provider"
  }
}
```

### Running the Application

```bash
dotnet run --project ExampleTemplate.WebApp
```

Navigate to `https://localhost:7177/swagger` to view the API documentation.

## API Endpoints

### Health Checks

- `GET /health` - Detailed health status with all checks
- `GET /health/live` - Liveness probe (simple check)
- `GET /health/ready` - Readiness probe (all checks)

### Controllers

Controllers are located in the `Controllers/` folder and follow the pattern:

```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
```

## Database Providers

### SQLite (Development)

Used by default for local development. No connection string required.

```json
{
  "Database": {
    "Provider": "sqlite"
  }
}
```

### PostgreSQL (Production)

```json
{
  "Database": {
    "Provider": "postgresql",
    "ConnectionString": "Host=localhost;Database=mydb;Username=user;Password=pass"
  }
}
```

## Middleware Pipeline

The application uses the following middleware pipeline:

1. CORS
2. HTTPS Redirection
3. Rate Limiting (if enabled)
4. Correlation ID
5. Authentication (if configured)
6. Authorization (if configured)
7. API Key (if configured)
8. Fallback Error Handler
9. Controllers

## Testing

Run tests with different configurations:

### Basic Tests (No Auth, No API Key)

```bash
dotnet test CleanWebApp/ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj
```

### Test with API Key Required

```bash
dotnet test CleanWebApp/ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj -p:DefineConstants="requireApiKey"
```

### Test with Authentication

```bash
dotnet test CleanWebApp/ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj -p:DefineConstants="includeAuth"
```

### Test with Both API Key and Authentication

```bash
dotnet test CleanWebApp/ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj -p:DefineConstants="requireApiKey%3BincludeAuth"
```

Note: Use `%3B` (URL-encoded semicolon) to separate multiple define constants, or use semicolon directly: `"requireApiKey;includeAuth"`.

## Conditional Compilation

The template supports conditional features via preprocessor directives:

- `#if includeAuth` - JWT authentication and authorization
- `#if requireApiKey` - API key middleware

## License

See the [LICENSE](../LICENSE) file for details.
