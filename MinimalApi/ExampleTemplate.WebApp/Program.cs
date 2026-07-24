#if (includeAuth)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ExampleTemplate.WebApp.Policies;
#endif
using ExampleTemplate.WebApp.Database.EntityFramework;
using ExampleTemplate.WebApp.Extensions;
using ExampleTemplate.WebApp.HealthChecks;
using ExampleTemplate.WebApp.Middleware;
using ExampleTemplate.WebApp.Models.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExampleTemplate.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Startup copy of the options for configuration
        var databaseOptions = new DatabaseSettings();
        builder.Configuration.GetSection("Database").Bind(databaseOptions);

        // Runtime copy of the options for the configuration
        builder.Services
            .AddOptions<DatabaseSettings>()
            .BindConfiguration("Database")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("allow-all", x => x
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());
        });

#if (includeAuth)
        var authOptions = new AuthenticationSettings();
            builder.Configuration.GetSection("Authentication").Bind(authOptions);

        builder.Services
            .AddOptions<AuthenticationSettings>()
            .BindConfiguration("Authentication")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
#if DEBUG
            x.RequireHttpsMetadata = false;
#endif

            x.MapInboundClaims = false;
            x.MetadataAddress = authOptions.WellKnownEndpoint;
            x.TokenValidationParameters = new()
            {
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer
            };
        });

        builder.Services
            .AddScoped<IAuthorizationHandler, AdminRequirementHandler>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNameConstants.ADMIN_POLICY, x =>
            {
                x.AddRequirements(new AdminRequirement());
            });
#endif

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .RegisterDatabaseContext(databaseOptions);

        // Configure health checks with database check
        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "database"]);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("allow-all");

        app.UseHttpsRedirection();

        app.UseMiddleware<CorrelationMiddleware>();

#if (includeAuth)
        app.UseAuthentication();
        app.UseAuthorization();
#endif

#if (requireApiKey)
        app.UseMiddleware<ApiKeyMiddleware>();
#endif
        app.UseMiddleware<FallbackMiddleware>();

        app.RegisterEndpoints();

        app.Run();
    }
}
