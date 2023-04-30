namespace Utilities.Billing.Api.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToDateTime(this long timestamp) => DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;

    public static DateTime ToDateTime(this ulong timestamp) => ToDateTime((long)timestamp);
}
