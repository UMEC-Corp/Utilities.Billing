using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
public static class Extensions
{
    public static IServiceCollection UseStellarWallets(this IServiceCollection services, ConfigurationManager configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<StellarWalletsSettings>(section);
        services.AddSingleton<IValidateOptions<StellarWalletsSettings>, StellarWalletsSettingsValidator>();
        services.AddOptionsWithValidateOnStart<StellarWalletsSettings>();

        return services.AddSingleton<IPaymentSystem, StellarWalletsClient>();
    }
}
