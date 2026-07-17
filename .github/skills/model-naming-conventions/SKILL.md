---
name: model-naming-conventions
description: 'Model naming conventions for this repository. Use when creating or reviewing models to ensure correct postfixes and prefixes are applied across all ICP.API projects.'
---

# Model Naming Conventions

All models across `API/src/` must follow these naming conventions. The name of a model communicates its purpose and where it belongs in the architecture. When creating any new model, consult this guide.

## Quick Reference

| Pattern | Example | Purpose | Layer |
|---------|---------|---------|-------|
| `*Entity` | `ShipmentEntity` | Entity Framework database model | Infrastructure |
| `Create*Request` | `CreateShipmentRequest` | HTTP request body for resource creation | Domain |
| `Update*Request` | `UpdateShipmentRequest` | HTTP request body for resource update | Domain |
| `*Dto` | `ShipmentDto` | Transformed domain data returned from controllers | Domain |
| `*QueryEnvelope` | `ShipmentQueryEnvelope` | Paginated GET request parameters | Domain |
| `*EnvelopeDto` | `ShipmentEnvelopeDto` | Paginated GET response body | Domain |
| `*Command` | `CreateShipmentCommand` | Message published to a queue | Infrastructure |
| `*Consumer` | `CreateShipmentConsumer` | Processes a queue `*Command` message | Infrastructure |
| `*Base` | `ShipmentBase` | Abstract base class (exempt from all other rules) | Any |

---

## Entity Models (`*Entity`)

Models used directly by Entity Framework to represent database tables or owned types must be postfixed with `Entity`.

- **Location:** `ICP.API.Infrastructure.*`
- **Postfix:** `Entity`

**Examples:**
```csharp
// Infrastructure layer
public class ShipmentEntity { ... }
public class BrokerageOrderEntity { ... }
```

---

## Request Models (`Create*Request` / `Update*Request`)

Models used to receive data from the body of an HTTP request must be postfixed with `Request`.

- **Location:** `ICP.API.Domain.*`
- **Postfix:** `Request`
- **Prefix rules:**
  - Models for **creating** a new resource are prefixed with `Create`
  - Models for **updating** an existing resource are prefixed with `Update`

**Examples:**
```csharp
// Domain layer
public class CreateShipmentRequest { ... }
public class UpdateShipmentRequest { ... }

public class CreateBrokerageOrderRequest { ... }
public class UpdateBrokerageOrderRequest { ... }
```

---

## Data Transfer Object Models (`*Dto`)

Models used to transform Entity Framework entities (or domain models) into a form suitable for the end user are postfixed with `Dto`.

- **Location:** `ICP.API.Domain.*`
- **Postfix:** `Dto`
- **Usage:** These are the **default return type** for all controller actions. Controllers must not return raw entities or infrastructure models.
- **Nested children:** Any child object nested inside a `*Dto` must also use the `Dto` postfix. Do not mix `*Dto`, `*Request`, or `*Response` types within the same object graph.

**Examples:**
```csharp
// Domain layer
public class ShipmentDto
{
    public AddressDto Origin { get; set; }       // ✓ child uses Dto postfix
    public ICollection<PackageDto> Packages { get; set; }  // ✓ collection children use Dto postfix
}

public class BrokerageOrderDto { ... }
```

---

## Paginated Request Models (`*QueryEnvelope`)

When a GET endpoint supports pagination, the model representing the incoming query parameters (page number, page size, filters, etc.) must be postfixed with `QueryEnvelope`.

- **Location:** `ICP.API.Domain.*`
- **Postfix:** `QueryEnvelope`

**Examples:**
```csharp
// Domain layer
public class ShipmentQueryEnvelope { ... }
public class BrokerageOrderQueryEnvelope { ... }
```

---

## Paginated Response Models (`*EnvelopeDto`)

When a GET endpoint returns a paginated result, the model wrapping the paged data (items, total count, page metadata, etc.) must be postfixed with `EnvelopeDto`.

- **Location:** `ICP.API.Domain.*`
- **Postfix:** `EnvelopeDto`

**Examples:**
```csharp
// Domain layer
public class ShipmentEnvelopeDto { ... }
public class BrokerageOrderEnvelopeDto { ... }
```

---

## Paginated Endpoint Pattern

A paginated GET endpoint uses both `*QueryEnvelope` and `*EnvelopeDto` together. Non-paginated endpoints may return `ICollection<*Dto>` instead of `*EnvelopeDto`.

```csharp
// Paginated — use *EnvelopeDto
[HttpGet]
public async Task<ActionResult<ShipmentEnvelopeDto>> GetShipments(
    [FromQuery] ShipmentQueryEnvelope query)
{
    ...
}

// Non-paginated — ICollection<*Dto> is acceptable
[HttpGet("all")]
public async Task<ActionResult<ICollection<ShipmentDto>>> GetAllShipments()
{
    ...
}
```

---

## Queue Models (`*Command` / `*Consumer`)

Models and classes used for queue-based messaging follow two conventions:

- **`*Command`** — The message published into the queue. Represents the intent to perform an action.
  - **Location:** `ICP.API.Infrastructure.*`
  - **Postfix:** `Command`
- **`*Consumer`** — The class that reads and processes a `*Command` from the queue.
  - **Location:** `ICP.API.Infrastructure.*`
  - **Postfix:** `Consumer`

Each `*Consumer` is responsible for processing exactly one matching `*Command`.

**Examples:**
```csharp
// Infrastructure layer
public class CreateShipmentCommand { ... }      // message sent to queue
public class CreateShipmentConsumer { ... }     // processes CreateShipmentCommand

public class UpdateBrokerageOrderCommand { ... }
public class UpdateBrokerageOrderConsumer { ... }
```

---

## Configuration Options (`*Options` / `IOptions<T>`)

Classes in `**/Options/**` folders are used exclusively to bind configuration data from `IConfiguration` at runtime startup. These classes are not subject to the Dto, Request, or Entity naming rules. They should be named after the configuration section they represent.

**Examples:**
```csharp
// Domain or Infrastructure layer
public class BrokerageApiOptions { ... }   // binds to "BrokerageApi" config section
public class ShippingClientOptions { ... }
```

---

## Abstract Base Classes (`*Base`)

Abstract base classes are exempt from all other naming rules. They must be postfixed with `Base` regardless of the layer they reside in.

**Examples:**
```csharp
public abstract class ShipmentBase { ... }
public abstract class BrokerageOrderBase { ... }
public abstract class ConsumerBase { ... }
```

---

## Common Pitfalls

- **Do not** return `*Entity` models from controllers — map them to a `*Dto` first.
- **Do not** use generic names like `ShipmentModel`, `ShipmentResponse`, or `ShipmentData` — always use the correct postfix.
- **Do not** use `*Request` as a return type — requests are inbound only.
- **Do not** place `*Entity` models in Domain or Application layers — they belong in Infrastructure.
- **Do not** place `*Dto`, `*Request`, `*QueryEnvelope`, or `*EnvelopeDto` models in Infrastructure — they belong in Domain.
- **Do not** nest a `*Request` or `*Response` type inside a `*Dto` — all nested children must also use the `Dto` postfix.
- **Do not** apply `Entity`, `Dto`, `Request`, `Command`, or `Consumer` postfixes to abstract base classes — use `Base` instead.
