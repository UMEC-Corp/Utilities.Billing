using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Utilities.Billing.Api.GrpcServices;
using Utilities.Billing.Api.OpenApi;
using Utilities.Billing.Data;
using Utilities.Billing.StellarWallets;
using Utilities.Common.Consul;
using Utilities.Common.Data;
using Utilities.Common.Grpc.Interceptors;

namespace Utilities.Billing.Api;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseOrleans(siloBuilder =>
        {
            siloBuilder.UseLocalhostClustering();
            siloBuilder.AddMemoryGrainStorage("urls");
        });

        builder.Services.UseStellarWallets();

        ConfigureConfiguration(builder);

        ConfigureLogging(builder);

        ConfigureGrpc(builder);

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

        ConfigureDataAccess(builder);

        ConfigureAuthentication(builder);

        var app = builder.Build();

        ConfigureEndpoints(app);

        await app.RunAsync();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration.GetValue<string>("Authentication:Authority");
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
                {
                    DefaultRequestVersion = HttpVersion.Version20,
                    Timeout = options.BackchannelTimeout,
                    MaxResponseContentBufferSize = 1024 * 1024 * 10, // 10 MB 
                };
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Microsoft ASP.NET Core OpenIdConnect handler");
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireBillingScope", policyBuilder => { policyBuilder.RequireClaim("scope", "billing"); });
        });
    }

    private static void ConfigureDataAccess(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<BillingDbContext>(o =>
        {
            o.UseNpgsql(builder.Configuration.GetConnectionString(nameof(BillingDbContext)))
                .UseSnakeCaseNamingConvention();
        });
        builder.Services.AddHostedService<DbContextMigrator<BillingDbContext>>();
    }

    private static void ConfigureGrpc(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerOptionsConfigurator>();

        builder.Services.AddGrpc(o =>
        {
            o.Interceptors.Add<ValidatingServerInterceptor>();
            o.Interceptors.Add<LoggingServerInterceptor>();
        }).AddJsonTranscoding();

        builder.Services.AddGrpcReflection();

        builder.Services.AddGrpcSwagger();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo { Title = "Billing V1", Version = "v1" });
        });
    }

    private static void ConfigureConfiguration(WebApplicationBuilder builder)
    {
        builder.Configuration.AddConsulConfiguration(builder.Environment.ApplicationName, builder.Environment.EnvironmentName);
    }

    private static void ConfigureEndpoints(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing V1"); });
        // Configure the HTTP request pipeline.
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<BillingService>();
            endpoints.MapGrpcService<AccountsService>();
            endpoints.MapGrpcReflectionService();
            endpoints.MapGet("/",
                () =>
                    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        });
    }

}