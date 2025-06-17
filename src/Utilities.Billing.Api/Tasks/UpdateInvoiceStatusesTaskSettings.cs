namespace Utilities.Billing.Api.Tasks;

/// <summary>
/// Configuration settings for the task that periodically updates invoice statuses.
/// </summary>
/// <remarks>
/// Inherits from <see cref="PeriodicTaskSettings"/> and provides a static <c>SectionName</c> property for configuration binding.
/// The settings typically include the <c>Period</c> property (inherited), which defines how often the task should run (in seconds or milliseconds, depending on implementation).
/// </remarks>
public class UpdateInvoiceStatusesTaskSettings : PeriodicTaskSettings
{
    /// <summary>
    /// The configuration section name used to bind settings from configuration sources.
    /// </summary>
    public static string SectionName = nameof(UpdateInvoiceStatusesTaskSettings);
}
