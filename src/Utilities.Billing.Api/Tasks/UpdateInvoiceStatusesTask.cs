using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Common.InitSequences;

namespace Utilities.Billing.Api.Tasks;

/// <summary>
/// Periodic background task that updates the statuses of invoices by querying the payment system for their latest state.
/// </summary>
/// <remarks>
/// This task runs at a configured interval (see <see cref="UpdateInvoiceStatusesTaskSettings"/>), and performs the following steps:
/// <list type="number">
///   <item>
///     <description>Waits for all initialization sequence steps to complete before proceeding.</description>
///   </item>
///   <item>
///     <description>Creates a new service scope and retrieves the <see cref="BillingDbContext"/> from the service provider.</description>
///   </item>
///   <item>
///     <description>Queries the database for all invoices with a status of <c>Pending</c>.</description>
///   </item>
///   <item>
///     <description>If there are no pending invoices, the task exits early.</description>
///   </item>
///   <item>
///     <description>Uses the <see cref="IPaymentSystem"/> to retrieve the latest status for each pending invoice.</description>
///   </item>
///   <item>
///     <description>Updates the status of each invoice in the database based on the information returned by the payment system.</description>
///   </item>
///   <item>
///     <description>Saves all changes to the database.</description>
///   </item>
/// </list>
/// The mapping from <see cref="PaymentSystemTransactionStatus"/> to <see cref="InvoiceStatus"/> is handled by the private <c>Map</c> method.
/// </remarks>
public class UpdateInvoiceStatusesTask : PeriodicBackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPaymentSystem _paymentSystem;
    private readonly IEnumerable<IInitSequenceStep> _initSequenceSteps;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateInvoiceStatusesTask"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency resolution.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The options monitor for task settings.</param>
    /// <param name="paymentSystem">The payment system used to query invoice statuses.</param>
    /// <param name="initSequenceSteps">The initialization sequence steps to wait for before execution.</param>
    public UpdateInvoiceStatusesTask(
        IServiceProvider serviceProvider,
        ILogger<UpdateInvoiceStatusesTask> logger,
        IOptionsMonitor<UpdateInvoiceStatusesTaskSettings> options,
        IPaymentSystem paymentSystem,
        IEnumerable<IInitSequenceStep> initSequenceSteps)
        : base(logger, options.CurrentValue)
    {
        _serviceProvider = serviceProvider;
        _paymentSystem = paymentSystem;
        _initSequenceSteps = initSequenceSteps;
    }

    /// <summary>
    /// Executes the periodic task to update invoice statuses.
    /// </summary>
    /// <remarks>
    /// Waits for all initialization steps to complete, then queries the database for pending invoices.
    /// For each pending invoice, retrieves the latest status from the payment system and updates the database accordingly.
    /// </remarks>
    protected override async Task Execute()
    {
        // Ensure that initialization was finished
        await _initSequenceSteps.WaitForCompletion();

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BillingDbContext>();

        var unfinishedInvoiceStatuses = new [] { InvoiceStatus.Pending };
        var unfinishedInvoices = await dbContext.Invoices.Where(x => unfinishedInvoiceStatuses.Contains(x.Status)).Select(x => x.Id).ToListAsync();
        if (!unfinishedInvoices.Any())
        {
            return;
        }

        var infos = await _paymentSystem.GetInvoicesInformationAsync(unfinishedInvoices);

        foreach (var info in infos)
        {
            await dbContext.Invoices.Where(x => x.Id == info.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(e => e.Status, Map(info.Status)));
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Maps a <see cref="PaymentSystemTransactionStatus"/> to an <see cref="InvoiceStatus"/>.
    /// </summary>
    /// <param name="status">The payment system transaction status.</param>
    /// <returns>The corresponding invoice status.</returns>
    /// <remarks>
    /// Returns <c>Completed</c> for <c>Success</c>, <c>Failed</c> for <c>Failed</c>, and <c>Unknown</c> for any other status.
    /// </remarks>
    private InvoiceStatus Map(PaymentSystemTransactionStatus status)
    {
        switch (status)
        {
            case PaymentSystemTransactionStatus.Success: return InvoiceStatus.Completed;
            case PaymentSystemTransactionStatus.Failed: return InvoiceStatus.Failed;
            default:
                return InvoiceStatus.Unknown;
        }
    }
}
