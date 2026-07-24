# Minimal API Template

A .NET template for creating minimal API applications with optional features.

## Features

This template supports the following optional features:

- **JWT Authentication** (`includeAuth`): Adds JWT bearer authentication and authorization
- **API Key Middleware** (`requireApiKey`): Requires an API key header for all requests

## Installation

Install the template:

```bash
dotnet new install .
```

## Usage

Create a new project from the template:

```bash
# Basic project
dotnet new minapi -n MyProject

# With JWT authentication
dotnet new minapi -n MyProject --includeAuth true

# With API key requirement
dotnet new minapi -n MyProject --requireApiKey true

# With both features
dotnet new minapi -n MyProject --includeAuth true --requireApiKey true
```

## Testing

### Running All Tests

```bash
cd MinimalApi
dotnet test ExampleTemplate.WebApp.Tests/
```

### Testing with API Key Middleware Enabled

Since the `requireApiKey` feature is controlled by preprocessor directives, you need to define the constant when testing:

```bash
cd MinimalApi
dotnet test ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj -p:DefineConstants="requireApiKey"
```

This enables the `ApiKeyMiddleware` in the test environment, allowing the `ApiKeyMiddlewareTests` to validate authentication properly.

### Testing with Authentication Enabled

```bash
cd MinimalApi
dotnet test ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj -p:DefineConstants="includeAuth"
```

### Testing with Both Features Enabled

```bash
cd MinimalApi
dotnet test ExampleTemplate.WebApp.Tests/ExampleTemplate.WebApp.Tests.csproj -p:DefineConstants="requireApiKey;includeAuth"
```

## Project Structure

```
ExampleTemplate.WebApp/
├── Constants/          # Application constants
├── Database/           # Entity Framework context and configuration
├── Endpoints/          # Minimal API endpoints
├── Extensions/         # Service registration extensions
├── Middleware/         # Custom middleware components
├── Models/             # DTOs and options
├── Policies/           # Authorization policies (if includeAuth=true)
├── Services/           # Business logic services
└── Validation/         # Custom validation attributes

ExampleTemplate.WebApp.Tests/
├── IntegrationTests/   # Integration tests with TestContainers
├── Setup/              # Test configuration and factories
└── UnitTests/          # Unit tests
```

## Configuration

Required environment variables for testing:

- `Database__ConnectionString`: PostgreSQL connection string
- `Database__Provider`: Database provider (e.g., "postgresql")
- `ApiKey`: API key value (when `requireApiKey=true`)

## Development

The template uses preprocessor directives (`#if`) to conditionally include features. This keeps the generated code clean when features are not needed.

When developing the template itself, you may need to define constants in your IDE or via MSBuild properties to enable specific features during development.
