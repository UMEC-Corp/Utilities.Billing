using FluentValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Utilities.Billing.Api.GrpcServices;
using Utilities.Billing.Api.OpenApi;
using Utilities.Billing.Data;
using Utilities.Billing.StellarWallets;
using Utilities.Common.Consul;
using Utilities.Common.Data;
using Utilities.Common.Grpc;
using Utilities.Common.Monitoring.HealthChecks;
using Utilities.Common.Monitoring.Metrics;

namespace Utilities.Billing.Api;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddConsulConfiguration(builder.Environment.ApplicationName, builder.Environment.EnvironmentName);

        builder.Host.UseOrleans(siloBuilder =>
        {
            siloBuilder.UseLocalhostClustering();
            siloBuilder.AddMemoryGrainStorageAsDefault();
        });

        builder.Services.UseStellarWallets(builder.Configuration, StellarWalletsSettings.SectionName);

        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.AddOpenTelemetryMetrics();


        ConfigureGrpc(builder);

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

        ConfigureDataAccess(builder);

        ConfigureAuthentication(builder);

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddWellKnownHealthChecks();

        var app = builder.Build();

        ConfigureEndpoints(app);

        await app.RunAsync();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.Configure<OAuth2IntrospectionOptions>(builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions)));

        builder.Services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
            .AddOAuth2Introspection(options =>
            {
                builder.Configuration.Bind(nameof(OAuth2IntrospectionOptions), options);
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireScope", policyBuilder => { policyBuilder.RequireClaim("scope", "billing"); });
        });
    }

    private static void ConfigureDataAccess(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<BillingDbContext>(o =>
        {
            o.UseNpgsql(builder.Configuration.GetConnectionString(nameof(BillingDbContext)))
                .UseSnakeCaseNamingConvention();
        });
        builder.Services.AddDbContextMigrator<BillingDbContext>();
    }

    private static void ConfigureGrpc(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerOptionsConfigurator>();

        builder.Services.AddManagedGrpc();

        builder.Services.AddGrpcSwagger();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo { Title = "Billing V1", Version = "v1" });
        });

    }

    private static void ConfigureEndpoints(WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing Api V1"); });

        app.UseReDoc();

        app.UseWellKnownHealthChecks();

        app.UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapGrpcService<BillingService>();
        app.MapGrpcService<AccountsService>();
        app.MapGrpcService<StellarService>();
        app.MapGrpcReflectionService();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
    }

}