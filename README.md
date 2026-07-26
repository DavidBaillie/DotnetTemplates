# .NET Templates

A collection of production-ready .NET project templates for rapid application development.

## Templates

### Minimal API Template

A modern ASP.NET Core Minimal API template with best practices, security features, and comprehensive testing setup.

**Location**: `MinimalApi/`

#### Features

- **Minimal API Architecture** - Lightweight, high-performance HTTP APIs
- **Multi-Database Support** - SQLite (in-memory) for development, PostgreSQL for production
- **Entity Framework Core** - Database access with migrations support
- **JWT Authentication** - Optional secure authentication with OpenID Connect
- **API Key Middleware** - Optional API key authentication
- **Health Checks** - Built-in health monitoring endpoints
- **CORS Configuration** - Configurable cross-origin resource sharing
- **Request Correlation** - Automatic request ID tracking for logging
- **Global Exception Handling** - Standardized error responses
- **OpenAPI/Swagger** - Automatic API documentation
- **Integration Testing** - Full test infrastructure with TestContainers
- **Custom Validation** - Reusable validation attributes

#### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop) (for PostgreSQL in tests)

#### Quick Start

1. **Clone or use the template**:
   ```bash
   # Navigate to the MinimalApi directory
   cd MinimalApi
   ```

2. **Configure the application**:
   ```bash
   # Copy the template settings
   cp ExampleTemplate.WebApp/appsettings.Template.json ExampleTemplate.WebApp/appsettings.Development.json
   
   # Edit appsettings.Development.json with your settings
   ```

3. **Run the application**:
   ```bash
   dotnet run --project ExampleTemplate.WebApp
   ```

4. **Access Swagger UI**:
   - Navigate to `https://localhost:7177/swagger`
   - Or `http://localhost:5294/swagger`

#### Configuration

The template uses `appsettings.json` for configuration. Key settings:

##### Database Configuration
```json
{
  "Database": {
    "Provider": "sqlite",           // or "postgresql"
    "ConnectionString": ""          // Required for postgresql
  }
}
```

##### CORS Configuration
```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"],
    "AllowedHeaders": ["Content-Type", "Authorization"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowCredentials": false
  }
}
```

##### Authentication (Optional)
```json
{
  "Authentication": {
    "WellKnownEndpoint": "https://your-idp/.well-known/openid-configuration",
    "Issuer": "https://your-idp"
  }
}
```

##### API Key (Optional)
```json
{
  "ApiKey": "your-secure-api-key-here"
}
```

#### Project Structure

```
ExampleTemplate.WebApp/
├── Constants/          # Application-wide constants
├── Database/          
│   └── EntityFramework/
│       ├── AppDbContext.cs           # Base DbContext
│       ├── SqliteAppDbContext.cs     # SQLite implementation
│       └── PostgreSql/
│           └── PostgresAppDbContext.cs
├── Endpoints/          # Minimal API endpoint handlers
├── Extensions/         # Service and middleware registration
├── Interfaces/         # Shared interfaces
├── Middleware/         # Custom middleware components
├── Models/
│   └── Options/        # Configuration option classes
├── Policies/           # Authorization policies
├── Validation/         # Custom validation attributes
└── Program.cs          # Application entry point

ExampleTemplate.WebApp.Tests/
├── Integration/        # Integration test lifecycle
└── Setup/             # Test configuration and factories
```

#### Endpoints

| Endpoint | Method | Description | Auth |
|----------|--------|-------------|------|
| `/health` | GET | Health check with details | Anonymous |
| `/health/live` | GET | Liveness probe | Anonymous |
| `/health/ready` | GET | Readiness probe | Anonymous |
| `/swagger` | GET | API documentation | Anonymous |

#### Database Migrations

When using PostgreSQL, manage migrations with EF Core:

```bash
# Create a migration
dotnet ef migrations add InitialCreate --project ExampleTemplate.WebApp

# Apply migrations
dotnet ef database update --project ExampleTemplate.WebApp

# Remove last migration (if not applied)
dotnet ef migrations remove --project ExampleTemplate.WebApp
```

**Note**: Set the `Database__ConnectionString` environment variable before running migrations:

```bash
# Windows PowerShell
$env:Database__ConnectionString="Host=localhost;Database=mydb;Username=user;Password=pass"

# Linux/Mac
export Database__ConnectionString="Host=localhost;Database=mydb;Username=user;Password=pass"
```

#### Running Tests

The template includes integration tests using TestContainers:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~IntegrationTestLifeCycle"
```

**Note**: Docker must be running for integration tests (they spin up a PostgreSQL container).

#### Security Features

- **Timing-Attack Resistant API Key Validation** - Uses constant-time comparison
- **JWT Bearer Authentication** - Industry-standard token authentication
- **CORS Protection** - Configurable cross-origin policies
- **Request Correlation** - Traceable requests for security auditing
- **Secure Exception Handling** - No sensitive data in responses (configurable)

#### Customization

##### Adding New Endpoints

Create endpoint classes in `Endpoints/` folder:

```csharp
public sealed class MyEndpoint
{
    public static IResult Get()
    {
        return TypedResults.Ok(new { message = "Hello World" });
    }
}
```

Register in `Extensions/MapEndpointsExtensions.cs`:

```csharp
endpoints.MapGet("/api/my-endpoint", MyEndpoint.Get)
    .WithName("GetMyEndpoint")
    .WithTags("MyEndpoints");
```

##### Adding Database Entities

1. Add entity classes to `Database/` folder
2. Add DbSet properties to `AppDbContext.cs`
3. Configure entity in `OnModelCreating()` method
4. Create and apply migration

```csharp
// In AppDbContext.cs
public DbSet<MyEntity> MyEntities => Set<MyEntity>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    modelBuilder.Entity<MyEntity>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
    });
}
```

##### Adding Custom Validation

Create validation attributes in `Validation/` folder:

```csharp
public sealed class MyValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Your validation logic
        return ValidationResult.Success;
    }
}
```

#### Production Considerations

Before deploying to production:

1. Update CORS policy to restrict origins
2. Configure PostgreSQL connection string securely (use secrets/key vault)
3. Disable `IncludeExceptionDetails` in configuration
4. Generate strong API keys if using API key authentication
5. Configure proper JWT issuer and audience validation
6. Enable HTTPS redirection
7. Configure rate limiting (consider adding)
8. Set up proper logging and monitoring
9. Review and update health check dependencies

#### Template Parameters

When creating a new project from this template, you can customize:

- `includeAuth` - Include JWT authentication infrastructure
- `requireApiKey` - Include API key middleware

Example:
```bash
dotnet new install ./MinimalApi
dotnet new minimalapi -n MyProject --includeAuth true --requireApiKey false
```

#### Troubleshooting

**Issue**: "Failed to find a valid database provider"
- **Solution**: Check `Database:Provider` in appsettings.json is either "sqlite" or "postgresql"

**Issue**: Integration tests fail with "Docker not found"
- **Solution**: Ensure Docker Desktop is running

**Issue**: PostgreSQL connection fails
- **Solution**: Verify connection string format and database accessibility

**Issue**: Migrations not applying
- **Solution**: Ensure `Database__ConnectionString` environment variable is set
