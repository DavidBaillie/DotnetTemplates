---
name: api-controllers
description: 'Conventions for building API controllers in this repository. Use when creating or reviewing controllers to ensure correct structure, attributes, routing, and response handling. Applies to all ICP.API.WebServices.* projects.'
---

# API Controller Conventions

This guide defines the conventions for building controllers across all `ICP.API.WebServices.*` projects. Always follow these rules when creating or reviewing controller code.

## Naming

- All controllers **must** use the `Controller` postfix (e.g., `ShipmentsController`, `LivingstonCLVSController`).

## Base Class

All controllers **must** inherit from `IcpControllerBase` (found in `ICP.API.WebServices.Common`), never from `ControllerBase` directly.

```csharp
public sealed class ShipmentsController(...) : IcpControllerBase
```

## Required Class-Level Attributes

Every controller must be decorated with both of the following attributes:

```csharp
[ApiController]
[ApiExplorerSettings(GroupName = "<group-name>")]
```

- **`[ApiController]`** — enables model binding, automatic 400 responses, and other MVC conventions.
- **`[ApiExplorerSettings(GroupName = "...")]`** — controls which Swagger/OpenAPI document the controller appears in. If you are unsure which group to use, **ask the developer before proceeding**.

## OpenAPI Method Decoration

Every action method must be decorated with the appropriate OpenAPI attributes. Use `System.Net.Mime.MediaTypeNames` constants rather than raw strings where possible.

```csharp
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType<MyResponseDto>(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

- `[Produces(...)]` — always declare the content type the action returns.
- `[Consumes(...)]` — always declare the content type the action accepts (required on POST/PUT/PATCH; omit on GET/DELETE).
- `[ProducesResponseType<T>(...)]` — use the generic form when a typed response body exists; use the non-generic form for status-only responses (e.g. 404, 204).
- Document **all** possible HTTP status codes the action can return.

## Return Types

All action methods must return either `ActionResult` or `ActionResult<T>`:

```csharp
// Untyped — suitable when multiple unrelated response types are possible
public async Task<ActionResult> GetAsync(...)

// Typed — preferred for the happy-path model when there is a single success type
public async Task<ActionResult<ShipmentDto>> GetAsync(...)
```

## GET by Id — Named Route Constant

When a controller exposes a GET-by-Id endpoint, it **must** be assigned a named route stored in a `private const string`:

```csharp
private const string GET_SHIPMENT = "GetShipment";

[HttpGet("{id:guid}", Name = GET_SHIPMENT)]
public async Task<ActionResult<ShipmentDto>> GetAsync(...)
```

This constant is reused in POST/PUT actions that return `CreatedAtRoute`.

## POST Methods — CreatedAtRoute

POST methods that create a resource must return `201 Created` using `CreatedAtRoute`, referencing the named GET route:

```csharp
[HttpPost]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType<MyResourceDto>(StatusCodes.Status201Created)]
public async Task<ActionResult> CreateAsync([FromBody] CreateMyResourceRequest request, CancellationToken cancellationToken)
{
    return (await myService.CreateAsync(request, cancellationToken)).Match<ActionResult>(
        (MyResourceDto dto) => CreatedAtRoute(
            routeName: GET_MY_RESOURCE,
            routeValues: new { id = dto.Id },
            value: dto),
        (Error<string> error) => InternalServerError()
    );
}
```

## PUT Methods — Route Id Authority

PUT methods must always take the resource `id` from the route and **never** trust an `id` provided inside the request body or update model. If the update model carries an `Id` property, ignore it and use the route value exclusively. If a developer received a model from a PUT endpoint's body and it contains a field called `Id`, ask the developer if the field can be removed.

```csharp
[HttpPut("{id:guid}")]
public async Task<ActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateMyResourceRequest request, ...)
{
    // Always use `id` from the route — never request.Id
}
```

## OneOf Return Types — Match Pattern

When a service method returns a `OneOf` discriminated union, always resolve it using the `.Match<ActionResult>(...)` pattern rather than switching on the type manually:

```csharp
return (await myService.GetAsync(id, cancellationToken)).Match<ActionResult>(
    (MyDto dto)    => Ok(dto),
    (NotFound _)   => NotFound(),
    (Error _)      => InternalServerError()
);
```

## No Business Logic in Controllers

Controllers must **only** translate HTTP input into service calls and map the result to the correct HTTP response. Do not place validation logic, domain calculations, or data transformations inside a controller action.

> **Exception:** When a trivial guard (e.g., a quick date-range or pagination check) would otherwise require a dedicated application service method, it may live in the controller. **Ask the developer before breaking this rule.**

## Input Model Validation

When an action accepts a model from the request body (`[FromBody]`) or query string (`[FromQuery]`), inspect every property of that model for validation attributes (e.g., `[Required]`, `[Range]`, `[StringLength]`).

If any property lacks a validation attribute, **ask the developer** whether this was intentional and whether a validation attribute should be added before proceeding.

The `[Required]` attribute should never be used on any value type that is not nullable (eg. `string`, `int`, `double`) The `[Required]` attribute checks for null, and when a non-nullable property is present, it will never trip the check. If a developer includes the attribute on a non-nullable property, inform them of this issue.

All `enum` properties should have the `[EnumIsDefined]` attribute. If a model is being validated and there is a property that is an `enum` and it does not have this attribute, add it and inform the developer.

## Full Example

```csharp
using System.Net.Mime;
using ICP.API.Application.Shipping.Interfaces.Shipment;
using ICP.API.Domain.Shipping.Models.Shipping;
using ICP.API.WebServices.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;

namespace ICP.API.WebServices.Shipping.Controllers.Shipments;

[ApiController]
[ApiExplorerSettings(GroupName = "shipping")]
[Route("api/v{version:apiVersion}/shipments")]
public sealed class ShipmentsController(IShipmentService shipmentService) : IcpControllerBase
{
    private const string GET_SHIPMENT_BY_ID = "GetShipmentById";

    [HttpGet("{id:guid}", Name = GET_SHIPMENT_BY_ID)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ShipmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShipmentDto>> GetAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return (await shipmentService.GetShipmentAsync(id, cancellationToken)).Match<ActionResult>(
            (ShipmentDto dto) => Ok(dto),
            (NotFound _)      => NotFound(),
            (Error _)         => InternalServerError()
        );
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ShipmentDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        return (await shipmentService.CreateShipmentAsync(request, cancellationToken)).Match<ActionResult>(
            (ShipmentDto dto) => CreatedAtRoute(GET_SHIPMENT, new { id = dto.Id }, dto),
            (Error _)         => InternalServerError()
        );
    }

    [HttpPut("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShipmentDto>> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        return (await shipmentService.UpdateShipmentAsync(id, request, cancellationToken)).Match<ActionResult>(
            (ShipmentDto dto)  => Ok(dto),
            (NotFound _) => NotFound(),
            (Error _)    => InternalServerError()
        );
    }
}
```
