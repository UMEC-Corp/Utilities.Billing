namespace Utilities.Billing.Api.Tasks;

/// <summary>
/// Represents configuration settings for a periodic background task.
/// </summary>
/// <remarks>
/// This class provides the <see cref="Period"/> property, which specifies the interval at which the periodic task should execute.
/// The <c>Period</c> value is typically interpreted as the number of seconds or milliseconds between task executions, depending on the consuming implementation.
/// </remarks>
public class PeriodicTaskSettings
{
    /// <summary>
    /// Gets or sets the interval for the periodic task execution.
    /// </summary>
    /// <remarks>
    /// Defines how often the background task should run. The unit (seconds or milliseconds) depends on the implementation that consumes this setting.
    /// </remarks>
    public int Period { get; set; }
}

