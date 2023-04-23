using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Utilities.Billing.Api.Interceptors;
using Utilities.Billing.Api.Services;
using Utilities.Billing.Data;
using Winton.Extensions.Configuration.Consul;
using BillingService = Utilities.Billing.Api.Services.BillingService;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
builder.Configuration
    .AddConsul($"common/appsettings.{builder.Environment.EnvironmentName}.json", options =>
    {
        options.ReloadOnChange = true;
        options.Optional = true;
    })
    .AddConsul($"{builder.Environment.ApplicationName}/appsettings.{builder.Environment.EnvironmentName}.json", options =>
    {
        options.ReloadOnChange = true;
        options.Optional = true;
    });

// Add services to the container.
builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<ValidatingServerInterceptor>();
    o.Interceptors.Add<LoggingServerInterceptor>();
}).AddJsonTranscoding();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "Billing V1", Version = "v1" });
});
builder.Services.AddDbContext<BillingDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString(nameof(BillingDbContext)))
        .UseSnakeCaseNamingConvention();
});
builder.Services.AddHostedService<BillingDbContextMigrator>();

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing V1");
});
// Configure the HTTP request pipeline.
app.MapGrpcService<BillingService>();
app.MapGrpcService<AccountsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
