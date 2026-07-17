# Entity Framework Repository Skill

This skill defines conventions for accessing the database via Entity Framework in all `ICP.API.*.Infrastructure` projects.

---

## Repository Classes

All database access via Entity Framework must be encapsulated in a class with the `Repository` postfix (e.g., `ShipmentRepository`, `BrokerageOrderRepository`).

Repositories are placed in the **Infrastructure Layer** of the relevant project.

---

## RepositoryBase

When implementing a repository, extend the `RepositoryBase<DbEntity, Dto>` or `RepositoryBase<DbEntity, Dto, CreateDto, UpdateDto>` class from `ICP.API.Infrastructure.Common` when you will be performing CRUD operations. If the repository does not require CRUD, do not inherit from this base class.

`RepositoryBase` provides a default implementation for the following CRUD operations:
- `CreateAsync`
- `GetAsync`
- `UpdateAsync`
- `DeleteAsync`
- `ExistsAsync`

Override any of these methods when the default behaviour is insufficient.

`RepositoryBase` requires two abstract methods to be implemented:
- `MapDtoToEntity` / `MapCreateRequestToEntity` — maps the application Dto to the database entity
- `MapEntityToDto` — maps the database entity back to the application Dto

When you implement the mapping in the abstract methods, prefer to use Mappery static mappers.
When any return type of the base implementation is insufficient, use the `new` keyworkd to override the definition. 

```csharp
new Task<OneOf<ShipmentDto, InvalidAction, NotFound>> GetAsync()
{
  // ...
}
```

---

## Repository Interfaces

Every repository **must** have a corresponding custom interface defined for it.

- Interfaces are stored in the **Application Layer** of the relevant project (e.g., `ICP.API.Application.Shipping`, `ICP.API.Application.Brokerage`).
- The interface **must** extend `IRepository<Dto>` or `IRepository<Dto, CreateDto, UpdateDto>` from `ICP.API.Application.Common.Interfaces` as appropriate.
- The interface name should use the `I` prefix and `Repository` postfix (e.g., `IShipmentRepository`).

---

## Creating a DbContext

Always create a DbContext via the appropriate factory and dispose of it with `using`:

```csharp
using var context = factory.Create*DbContext();
```

### Shipping Project (`ICP.API.Infrastructure.Shipping`)

Use `IPooledAppDbContextFactory` to create a `DbContext`:

```csharp
using var context = factory.CreateAppDbContext();
```

> **Important:** `CreateUnfilteredAppDbContext()` bypasses the customer filter and allows cross-client data access. Only use this method with **express permission from the developer**.

### Brokerage Project (`ICP.API.Infrastructure.Brokerage`)

Use `IPooledBrokerageDbContextFactory` to create a `DbContext`:

```csharp
using var context = factory.CreateBrokerageDbContext();
```

---

## Summary Checklist

| Rule | Detail |
|------|--------|
| Class naming | Must use `Repository` postfix |
| Base class | Extend `RepositoryBase<DbEntity, Dto>` or `RepositoryBase<DbEntity, Dto, CreateDto, UpdateDto>` |
| Interface | Required; stored in the Application Layer; extends `IRepository<...>` |
| Interface naming | Must use `I` prefix and `Repository` postfix |
| DbContext (Shipping) | `factory.CreateAppDbContext()` via `IPooledAppDbContextFactory` |
| DbContext (Brokerage) | `factory.CreateBrokerageDbContext()` via `IPooledBrokerageDbContextFactory` |
| Unfiltered context | Only with explicit developer approval |
| DbContext lifetime | Always use `using var context = ...` |
