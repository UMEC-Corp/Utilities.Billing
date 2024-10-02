using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Utilities.Billing.Api.Tasks
{
    public class UpdateInvoiceStatusesTaskSettings : PeriodicTaskSettings
    {
        public static string SectionName = nameof(UpdateInvoiceStatusesTaskSettings);
    }
}