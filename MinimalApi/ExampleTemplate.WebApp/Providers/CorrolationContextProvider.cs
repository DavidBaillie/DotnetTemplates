#pragma warning disable IDE1006

using ExampleTemplate.WebApp.Interfaces;
using ExampleTemplate.WebApp.Models.Context;

namespace ExampleTemplate.WebApp.Providers;

public sealed class CorrelationContextProvider : IProvider<CorrelationContext>
{
    private Lazy<CorrelationContext> correlationContext { get; set; } =
        new(() => new CorrelationContext() { CorrelationId = string.Empty }, true);

    public CorrelationContext Get() => correlationContext.Value;
}