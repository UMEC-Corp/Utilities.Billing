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

        var imageRegistry = Environment.GetEnvironmentVariable("TEST_CONTAINERS_IMAGE_REGISTRY") ?? "registry.digitalocean.com/deviot";
        var imageTag = Environment.GetEnvironmentVariable("TEST_CONTAINERS_IMAGE_TAG") ?? "develop-latest";
        var imageName = $"{imageRegistry}/utilities-billing:{imageTag}";

        var builder = new ContainerBuilder().WithImage(imageName)
            .WithNetwork(context.Network)
            .WithName("billing")
            .WithPortBinding(ServiceConstants.DefaultGrpcPort, true)
            .WithPortBinding(ServiceConstants.DefaultRestPort, true)
            .WithEnvironment(context.CommonSettings)
            .WithEnvironment(settings)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ServiceConstants.DefaultGrpcPort));

        var container = builder.Build();

        try
        {
            await container.StartAsync();
        }
        catch (Exception ex)
        {
            var logDetails = string.Empty;
            try
            {
                var (stdout, stderr) = await container.GetLogsAsync();
                logDetails = $"\n--- STDOUT ---\n{stdout}\n--- STDERR ---\n{stderr}";
            }
            catch
            {
                // Container was never created (e.g. image pull failed) — logs unavailable
            }

            throw new Exception(
                $"Container 'billing' failed to start: {ex.Message}{logDetails}", ex);
        }

        var service = new GrpcService(container);
        context.AddService(service);
        return service;
    }
}
