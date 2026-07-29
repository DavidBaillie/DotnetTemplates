# .NET Templates Collection

[![Test Template Configurations](https://github.com/DavidBaillie/DotnetTemplates/actions/workflows/test-template.yml/badge.svg)](https://github.com/DavidBaillie/DotnetTemplates/actions/workflows/test-template.yml)
[![Quick Template Validation](https://github.com/DavidBaillie/DotnetTemplates/actions/workflows/quick-validation.yml/badge.svg)](https://github.com/DavidBaillie/DotnetTemplates/actions/workflows/quick-validation.yml)

A curated collection of .NET project templates designed to accelerate development with best practices, common patterns, and optional features built-in.

## Available Templates

### MinimalApi

A comprehensive .NET Minimal API template with built-in support for:
- JWT Authentication (optional)
- API Key Middleware (optional)
- Entity Framework Core with PostgreSQL
- Health checks
- Rate limiting
- CORS configuration
- Integration and unit testing setup

[View MinimalApi Documentation →](./MinimalApi/README.md)

### WebApi

A production-ready ASP.NET Core Web API template using the **Controller pattern** with:
- Controller-Based Architecture (traditional MVC-style)
- JWT Authentication & Authorization (optional)
- API Key Middleware (optional)
- Entity Framework Core with SQLite (dev) and PostgreSQL (production)
- Health checks and monitoring
- Rate limiting with IP-based partitioning
- Swagger/OpenAPI documentation
- Comprehensive testing setup

[View WebApi Documentation →](./WebApi/README.md)

## Quick Start

### Installation

Install a template locally for use with `dotnet new`:

```bash
# Navigate to the template directory
cd MinimalApi  # or cd WebApi

# Install the template
dotnet new install .
```

### Usage

Once installed, create a new project from any template:

```bash
# Create a new Minimal API project
dotnet new minapi -n MyApiProject

# With optional features
dotnet new minapi -n MyApiProject --includeAuth true --requireApiKey true

# Create a new Controller-based Web API project
dotnet new starterwebapp -n MyWebApi

# With optional features
dotnet new starterwebapp -n MyWebApi --includeAuth true --requireApiKey true
```

### Uninstalling Templates

To remove an installed template:

```bash
dotnet new uninstall <path-to-template-directory>
```

## Template Structure

Each template in this repository follows a consistent structure:

```
TemplateName/
├── README.md                          # Template-specific documentation
├── template.json or .template.config  # Template configuration
├── *.sln or *.slnx                   # Solution file
└── [Project directories]              # Template source code
```

## Development

### Adding a New Template

1. Create a new directory for your template
2. Add a `template.json` or `.template.config/template.json` file with template metadata
3. Create a comprehensive README.md documenting features and usage
4. Update this main README to list the new template

### Testing Templates

Before publishing, test each template:

```bash
# Install locally
dotnet new install ./TemplateName

# Create a test project
dotnet new <short-name> -n TestProject

# Build and run tests
cd TestProject
dotnet build
dotnet test
```

### Automated Testing

This repository includes GitHub Actions workflows for continuous integration:

#### Quick Validation (`quick-validation.yml`)
- Runs on every push and pull request
- Validates template syntax
- Tests template installation
- Builds projects with default and all-features configurations
- Fast feedback (< 5 minutes)

#### Comprehensive Testing (`test-template.yml`)
- Tests all parameter combinations (4 combinations per template)
- Runs on both Ubuntu and Windows runners
- Executes full test suite for each configuration
- Uploads test results as artifacts
- Comprehensive quality assurance (~ 15-20 minutes)

The workflows automatically test every possible combination of template parameters to ensure all configurations work correctly.