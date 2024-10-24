using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.TonWallets.Extensions;

public static class Extensions
{
    public static IServiceCollection UseTonWallets(this IServiceCollection services, ConfigurationManager configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<TonWalletsSettings>(section);
        services.AddSingleton<IValidateOptions<TonWalletsSettings>, TonWalletsSettingsValidator>();
        services.AddOptionsWithValidateOnStart<TonWalletsSettings>();

        return services.AddSingleton<IPaymentSystem, TonWalletsClient>();
    }
}
