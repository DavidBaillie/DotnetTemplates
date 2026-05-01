using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Extensions;
using SukkotStore.WebApp.Middleware;
using SukkotStore.WebApp.Models.Options;
using SukkotStore.WebApp.Policies;

namespace SukkotStore.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Startup copy of the options for configuration
        var databaseOptions = new DatabaseSettings();
        builder.Configuration.GetSection("Database").Bind(databaseOptions);

        var authOptions = new AuthenticationSettings();
        builder.Configuration.GetSection("Authentication").Bind(authOptions);

        // Runtime copy of the options for the configuration
        builder.Services
            .AddOptions<DatabaseSettings>()
            .BindConfiguration("Database")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<AuthenticationSettings>()
            .BindConfiguration("Authentication")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("allow-all", x => x
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());
        });

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

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .RegisterDatabaseContext(databaseOptions)
            .RegisterHttpClients(authOptions);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("allow-all");

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.RegisterEndpoints();
        app.MapControllers();

        app.UseMiddleware<FallbackMiddleware>()
            .UseMiddleware<ApiKeyMiddleware>();

        app.Run();
    }
}
