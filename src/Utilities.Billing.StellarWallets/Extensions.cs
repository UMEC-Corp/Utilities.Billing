using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
public static class Extensions
{
    public static IServiceCollection UseStellarWallets(this IServiceCollection services, ConfigurationManager configuration)
    {
        var section = configuration.GetSection(nameof(StellarWalletsSettings));
        services.Configure<StellarWalletsSettings>(section);

        return services.AddSingleton<IPaymentSystem, StellarWalletsClient>();
    }
}
