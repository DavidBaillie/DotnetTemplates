---
name: controller-service-workflow
description: "Use when working with a controller that performs actions against the database, CRUD endpoints, database-backed controller actions, service-layer orchestration, repository validation cleanup, or Clean Architecture controller-to-service-to-repository workflows in ICP.API projects."
---

# Controller Service Workflow

Use this skill whenever creating, updating, or reviewing a controller action that performs database-backed work. The expected flow is:

```text
Controller -> Application service interface -> Application service -> Application repository interface -> Infrastructure repository -> database
```

This keeps HTTP concerns in the Presentation layer, business decisions in the Application layer, and database persistence in the Infrastructure layer.

## Controller Rules

- Controllers must depend on service interfaces, not repository interfaces.
- Controllers translate HTTP input into service calls and map service results into HTTP responses.
- Controllers should not perform business validation, database checks, state-transition decisions, or persistence logic.
- If an existing controller injects a repository for CRUD/database actions, refactor it to inject a service interface instead.
- Keep controller result mapping stable when introducing the service layer unless the user explicitly asks for behavior changes.

Example controller dependency:

```csharp
public sealed class ExampleController(IExampleService service) : IcpControllerBase
```

## Service Rules

- Every database-backed controller workflow should have a service class in the Application layer.
- Every service class must have a matching interface in the Application layer.
- Service interfaces use the `I` prefix and `Service` postfix, for example `IInternationalCarrierService`.
- Service implementations use the `Service` postfix, for example `InternationalCarrierService`.
- Service classes perform all business validation and orchestration.
- Service classes decide whether repository query results mean `InvalidAction`, `NotFound`, `Success`, `Error<T>`, or another application outcome.
- Service methods may forward simple read/create operations directly when no business validation is required.
- Service classes should default to `AddTransient<IService, Service>()` registration.

Typical service responsibilities:

- Check whether a record exists before update or delete.
- Check whether a unique code/name/number is already used based on the DbContext configuration.
- Check whether related records prevent a state change.
- Convert repository CRUD/query results into application-level `OneOf` outcomes.

## Repository Rules

- Every repository must have a matching interface in the Application layer.
- Repository interfaces use the `I` prefix and `Repository` postfix, for example `IInternationalCarrierRepository`.
- Repository implementations live in the Infrastructure layer and use the `Repository` postfix.
- Repositories should default to `AddTransient<IRepository, Repository>()` registration unless an existing local pattern requires another lifetime.
- Repositories perform database persistence and database queries only.
- Repositories should not perform business validation or decide whether an operation is allowed.
- Repositories may expose small helper query methods needed by services, such as `ExistsWithCodeAsync`, `HasShipperProfiles`, or `ExistsActiveAsync`.
- Helper methods should answer database facts, not encode business decisions.

Good repository helper shape:

```csharp
Task<bool> HasShipperProfiles(Guid id, CancellationToken cancellationToken);
```

Avoid repository methods that return business outcomes for validation rules:

```csharp
Task<OneOf<Success, InvalidAction, NotFound>> DeleteAsync(Guid id, CancellationToken cancellationToken);
```

Prefer keeping the repository delete/update CRUD-shaped and let the service produce `InvalidAction` when helper query results require it.

## Refactor Checklist

When moving an existing controller/repository workflow into this pattern:

1. Add `I<Entity>Service` in the Application layer.
2. Add `<Entity>Service` in the Application layer.
3. Move validation and state-transition rules from the repository into the service.
4. Add repository helper query methods for database facts the service needs.
5. Simplify repository CRUD methods so they only persist, retrieve, update, delete, or return database errors.
6. Update the controller to inject and call the service interface.
7. Register both service and repository with transient lifetimes by default.
8. Run focused controller/service tests that cover happy paths and moved validation rules.

## Validation Placement Guide

| Rule type | Location |
|-----------|----------|
| HTTP route/body binding | Controller |
| Data annotation model validation | Domain request model / ASP.NET model validation |
| Business rule validation | Application service |
| Existence checks used for business decisions | Repository helper queried by service |
| Relationship checks used for business decisions | Repository helper queried by service |
| EF Core persistence errors | Repository |
| HTTP status mapping | Controller |

## International Carrier Example

The International Carrier workflow is the reference pattern for this skill:

- `InternationalCarrierController` injects `IInternationalCarrierService`.
- `IInternationalCarrierService` lives in `ICP.API.Application.Shipping.Interfaces.Customers`.
- `InternationalCarrierService` lives in `ICP.API.Application.Shipping.Services`.
- `InternationalCarrierService` performs validation such as preventing deactivation or deletion when shipper profiles are attached.
- `IInternationalCarrierRepository` exposes helper facts such as `HasShipperProfiles` and `ExistsActiveAsync`.
- `InternationalCarrierRepository` performs CRUD and helper queries only.
- Both `IInternationalCarrierService` and `IInternationalCarrierRepository` are registered as transient dependencies.