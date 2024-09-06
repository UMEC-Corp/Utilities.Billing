using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.Billing.StellarWallets
{
    internal class StellarWalletsSettingsValidator : IValidateOptions<StellarWalletsSettings>
    {
        private IConfigurationSection _configSection;

        public StellarWalletsSettingsValidator(IConfiguration config)
        {
            _configSection = config.GetSection(StellarWalletsSettings.SectionName);
        }

        public ValidateOptionsResult Validate(string? name, StellarWalletsSettings options)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(options.HorizonUrl))
            {
                errors.Add($"Configuration parameter '{_configSection.Path}:{nameof(options.HorizonUrl)}' has no value!");
            }

            if (string.IsNullOrEmpty(options.SecretSeed))
            {
                errors.Add($"Configuration parameter '{_configSection.Path}:{nameof(options.SecretSeed)}' has no value!");
            }

            if (errors.Any())
            {
                return ValidateOptionsResult.Fail(string.Join("\n", errors));
            }

            return ValidateOptionsResult.Success;
        }
    }
}
