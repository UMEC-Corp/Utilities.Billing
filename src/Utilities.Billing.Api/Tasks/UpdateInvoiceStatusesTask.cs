using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.StellarWallets;

namespace Utilities.Billing.Api.Tasks
{

    public class UpdateInvoiceStatusesTask : PeriodicBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPaymentSystem _paymentSystem;

        public UpdateInvoiceStatusesTask(IServiceProvider serviceProvider, ILogger<UpdateInvoiceStatusesTask> logger, IOptionsMonitor<UpdateInvoiceStatusesTaskSettings> options, IPaymentSystem paymentSystem)
            : base(logger, options.CurrentValue)
        {
            _serviceProvider = serviceProvider;
            _paymentSystem = paymentSystem;
        }

        protected override async Task Execute()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<BillingDbContext>();

            var unfinishedInvoiceStatuses = new InvoiceStatus[] { InvoiceStatus.Pending };
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

        private InvoiceStatus Map(PaymentSystemTransactionStatus status)
        {
            switch (status)
            {
                case PaymentSystemTransactionStatus.Success: return InvoiceStatus.Completed;
                case PaymentSystemTransactionStatus.Failed: return InvoiceStatus.Failed;
                default:
                    return InvoiceStatus.Unknow;
            }
        }
    }

}
