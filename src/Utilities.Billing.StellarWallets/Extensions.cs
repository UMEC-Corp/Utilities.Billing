using Microsoft.Extensions.DependencyInjection;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
public static class Extensions
{
    public static IServiceCollection UseStellarWallets(this IServiceCollection services, StellarWalletsSettings? settings = default)
    {
        if (settings != null)
        {
            services.ConfigureOptions(settings);
        }
        return services.AddSingleton<IPaymentSystem, StellarWalletsClient>();
    }
}
