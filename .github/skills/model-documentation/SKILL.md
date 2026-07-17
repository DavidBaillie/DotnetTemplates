---
name: model-documentation
description: 'Conventions for documenting code across all ICP.API projects. Use when writing or reviewing interfaces, concrete implementations, and Domain Dto models to ensure consistent XML documentation is applied.'
---

# Code Documentation Conventions

All projects under `API/src/` follow a consistent XML documentation strategy. Apply these rules whenever adding documentation to files with the `.cs` extension.

---

## Interfaces

Every interface **must** have a `<summary>` tag that describes its purpose and intended use.

```csharp
/// <summary>
/// Defines the contract for retrieving and persisting shipment data.
/// Used by the application layer to decouple business logic from the underlying data store.
/// </summary>
public interface IShipmentRepository
{
    // ...
}
```

Every method **defined** on an interface **must** also have a `<summary>` tag describing what the method does.

```csharp
/// <summary>
/// Defines the contract for retrieving and persisting shipment data.
/// Used by the application layer to decouple business logic from the underlying data store.
/// </summary>
public interface IShipmentRepository
{
    /// <summary>
    /// Retrieves a shipment by its unique identifier.
    /// </summary>
    Task<ShipmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new shipment record to the data store.
    /// </summary>
    Task AddAsync(ShipmentDto shipment, CancellationToken cancellationToken);
}
```

---

## Concrete Implementations

Concrete classes that implement an interface **must** use the `<inheritdoc />` tag at the class level. This automatically inherits all documentation from the interface — do not duplicate it.

```csharp
/// <inheritdoc />
public class ShipmentRepository : IShipmentRepository
{
    // ...
}
```

Method implementations inherit their documentation from the interface via the class-level `<inheritdoc />`. If a method implementation has behaviour worth calling out (e.g. caching strategy, retry logic, side effects), you may add a `<remarks>` tag to the method to provide supplementary detail without overriding the inherited summary.

```csharp
/// <inheritdoc />
/// <remarks>
/// This repository is important and does caching.
/// </remarks>
public class ShipmentRepository : IShipmentRepository
{
    /// <inheritdoc />
    /// <remarks>
    /// Results are cached for 5 minutes using the shipment ID as the cache key.
    /// </remarks>
    public async Task<ShipmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // ...
    }
}
```

Do **not** add a `<summary>` tag to methods on a concrete class — use `<remarks>` only for supplementary information.
If you find existing code with the `<summary>` tag, do not remove the comment as it is important and has been added by a developer.

---

## Domain Dto Models

All models with the `Dto` postfix located inside a `ICP.API.Domain.*` project **must** be fully documented.

### Class-level Documentation

The class must have a `<summary>` tag describing its purpose — what it represents and how it is used.

```csharp
/// <summary>
/// Represents the core data for a shipment as it moves through the application layer.
/// Used to transfer shipment information between services without exposing domain entities directly.
/// </summary>
public class ShipmentDto
{
    // ...
}
```

### Property-level Documentation

Every property on the Dto **must** have a `<summary>` tag.

```csharp
/// <summary>
/// Represents the core data for a shipment as it moves through the application layer.
/// Used to transfer shipment information between services without exposing domain entities directly.
/// </summary>
public class ShipmentDto
{
    /// <summary>
    /// The UUID for the shipment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The tracking number assigned by the carrier.
    /// </summary>
    public string TrackingNumber { get; set; } = string.Empty;

    /// <summary>
    /// The current status of the shipment.
    /// </summary>
    public ShipmentStatus Status { get; set; }
}
```

---

## OneOf Return Types

When a method returns a `OneOf<T0, T1, ...>` discriminated union, the `<returns>` tag **must** describe every possible case using a `<list type="bullet">` block. Each item should name the type and describe when it is returned.

```csharp
/// <summary>
/// Attempts to process the payment for the given order.
/// </summary>
/// <param name="orderId">The unique identifier of the order to process.</param>
/// <returns>
/// <list type="bullet">
/// <item><term><see cref="PaymentResultDto"/></term><description>The payment was processed successfully.</description></item>
/// <item><term><see cref="InsufficientFunds"/></term><description>The account did not have enough balance to cover the order total.</description></item>
/// <item><term><see cref="Error"/></term><description>An unexpected error occurred during processing.</description></item>
/// </list>
/// </returns>
OneOf<PaymentResultDto, InsufficientFunds, Error> ProcessPayment(Guid orderId);
```

Apply this pattern on:
- Interface method definitions
- Concrete method overrides that add a `<remarks>` tag (place the `<returns>` tag inside the `<remarks>` block in that case)

Do **not** use a plain prose sentence for `OneOf` returns — always use the structured list so every outcome is explicitly documented.

---

## Quick Reference

| Code Element | Required Documentation |
|---|---|
| Interface | `<summary>` on the interface itself |
| Interface method | `<summary>` on each method definition |
| Concrete class (implements interface) | `<inheritdoc />` on the class |
| Concrete method (extra detail needed) | `<remarks>` on the method only |
| Domain `*Dto` class | `<summary>` on the class |
| Domain `*Dto` property | `<summary>` on every property |
