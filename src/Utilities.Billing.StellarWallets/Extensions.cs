using Microsoft.Extensions.DependencyInjection;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
public static class Extensions
{
    public static IServiceCollection UseStellarWallets(this IServiceCollection services)
    {
        return services.AddSingleton<IPaymentSystem>(new StellarWalletsClient());
    }
}
