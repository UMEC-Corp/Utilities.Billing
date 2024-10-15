using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Utilities.Billing.TonWallets;

internal class TonWalletsSettingsValidator : IValidateOptions<TonWalletsSettings>
{
    private IConfigurationSection _configSection;

    public TonWalletsSettingsValidator(IConfiguration config)
    {
        _configSection = config.GetSection(TonWalletsSettings.SectionName);
    }

    public ValidateOptionsResult Validate(string? name, TonWalletsSettings options)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(options.Endpoint))
        {
            errors.Add($"Configuration parameter '{_configSection.Path}:{nameof(options.Endpoint)}' has no value!");
        }

        if (string.IsNullOrEmpty(options.ApiKey))
        {
            errors.Add($"Configuration parameter '{_configSection.Path}:{nameof(options.ApiKey)}' has no value!");
        }

        if (string.IsNullOrEmpty(options.MasterSecret))
        {
            errors.Add($"Configuration parameter '{_configSection.Path}:{nameof(options.MasterSecret)}' has no value!");
        }

        if (errors.Any())
        {
            return ValidateOptionsResult.Fail(string.Join("\n", errors));
        }

        return ValidateOptionsResult.Success;
    }
}