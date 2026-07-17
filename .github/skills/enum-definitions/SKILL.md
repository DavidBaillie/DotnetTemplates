---
name: enum-definitions
description: 'Conventions for defining enums in this repository. Use when creating or reviewing enums to ensure correct value assignment, serialization attributes, and range spacing. Applies to all ICP.API projects.'
---

# Enum Definition Conventions

This guide defines the conventions for declaring enums across all `ICP.API.*` projects. Always follow these rules when creating or reviewing enum code.

## Integer Values

- Every enum member **must** have an explicit integer value assigned.
- `0` is **never** a valid enum value. Do not define any member with a value of `0`.

## Value Spacing

- Enum values **must** start at a multiple of `1000` (e.g. `1000`, `2000`, `3000`).
- Each logical group of values begins at the next multiple of `1000`.
- Developers **may** insert new values between multiples; use the next available integer within the block.

```csharp
public enum ShipmentStatus
{
    // 1000 block — pending states
    Pending        = 1000,
    PendingReview  = 1500,

    // 2000 block — active states
    InTransit      = 2000,
    OutForDelivery = 2500,

    // 3000 block — terminal states
    Delivered      = 3000,
    Cancelled      = 3500,
    Failed         = 3750,
}
```

## JSON Serialization Attribute

Every enum **must** be decorated with `[JsonConverter]` so that it serializes as a string rather than an integer:

```csharp
[JsonConverter(typeof(JsonStringEnumConverter<ShipmentStatus>))]
public enum ShipmentStatus
{
    Pending   = 1000,
    InTransit = 2000,
    Delivered = 3000,
}
```

- Replace `ShipmentStatus` in the attribute with the actual enum name.
- The attribute requires `System.Text.Json.Serialization` to be in scope.
