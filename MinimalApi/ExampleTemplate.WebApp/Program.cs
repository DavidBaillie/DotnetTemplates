#if includeAuth
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ExampleTemplate.WebApp.Policies;
#endif
using ExampleTemplate.WebApp.Constants;
using ExampleTemplate.WebApp.Database.EntityFramework;
using ExampleTemplate.WebApp.Extensions;
using ExampleTemplate.WebApp.HealthChecks;
using ExampleTemplate.WebApp.Middleware;
using ExampleTemplate.WebApp.Models.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.RateLimiting;

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

#if includeAuth
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
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Example Template API",
                Version = "v1",
                Description = "A Minimal API template with best practices for ASP.NET Core",
            });

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Group endpoints by tags
            options.TagActionsBy(api => [api.GroupName ?? "Default"]);
            options.DocInclusionPredicate((name, api) => true);
        });

        // Configure rate limiting
        var rateLimitOptions = new RateLimitSettings();
        builder.Configuration.GetSection("RateLimit").Bind(rateLimitOptions);

        builder.Services
            .AddOptions<RateLimitSettings>()
            .BindConfiguration("RateLimit")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        if (rateLimitOptions.Enabled)
        {
            builder.Services.AddRateLimiter(options =>
            {
                // Default policy - fixed window
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    // Partition by IP address
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitOptions.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitOptions.WindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = rateLimitOptions.QueueLimit
                    });
                });

                // Customize rejection response
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    TimeSpan? retryAfter = null;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue))
                    {
                        retryAfter = retryAfterValue;
                        context.HttpContext.Response.Headers.RetryAfter = retryAfterValue.TotalSeconds.ToString();
                    }

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests",
                        message = "Rate limit exceeded. Please try again later.",
                        retryAfter = retryAfter?.TotalSeconds
                    }, cancellationToken);
                };

                // Named policy for more restrictive endpoints
                options.AddPolicy("strict", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
                });
            });
        }

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

        // Enable rate limiting if configured
        if (rateLimitOptions.Enabled)
        {
            app.UseRateLimiter();
        }

        app.UseMiddleware<CorrelationMiddleware>();

#if includeAuth
        app.UseAuthentication();
        app.UseAuthorization();
#endif

#if requireApiKey
        app.UseMiddleware<ApiKeyMiddleware>();
#endif
        app.UseMiddleware<FallbackMiddleware>();

        app.RegisterEndpoints();

        app.Run();
    }
}
