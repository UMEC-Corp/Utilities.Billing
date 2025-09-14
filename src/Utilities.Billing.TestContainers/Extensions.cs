using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Utilities.Common.TestTools;

namespace Utilities.Billing.TestContainers;

public static class Extensions
{
    public static async Task<IGrpcService> AddBilling(this ServiceTestContext context)
    {
        if (context.IsLocal)
        {
            return context.GetLocalGrpcService("https://localhost:7178"); // Adjust port if needed
        }

        // Add dependencies if required (e.g., Postgres, Redis)
        var postgresConnectionString = await context.AddPostgresAsync("billing");
        await context.AddRedisAsync();
        await context.AddRabbitMqAsync();

        context.PublishCommonSettings(new Dictionary<string, string>
        {
            { "ManagedServices__Billing__EndpointUrl", $"http://billing:{ServiceConstants.DefaultGrpcPort}" },
        });

        var settings = new Dictionary<string, string>
        {
            // Add service-specific environment variables here
             { "ConnectionStrings__BillingDbContext", postgresConnectionString },
        };

        var builder = new ContainerBuilder().WithImage("deviot.azurecr.io/utilities-billing:latest")
            .WithNetwork(context.Network)
            .WithName("billing")
            .WithPortBinding(ServiceConstants.DefaultGrpcPort, true)
            .WithPortBinding(ServiceConstants.DefaultRestPort, true)
            .WithEnvironment(context.CommonSettings)
            .WithEnvironment(settings)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ServiceConstants.DefaultGrpcPort));

        var container = builder.Build();
        await container.StartAsync();

        var service = new GrpcService(container);
        context.AddService(service);
        return service;
    }
}
