using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Grains
{
    public class DeviceGrain : Grain, IDeviceGrain
    {
        private readonly BillingDbContext _dbContext;
        private readonly IPaymentSystem _paymentSystem;
        private readonly ILogger<DeviceGrain> _logger;
        private Dictionary<string, InputInfo> _inputStates;

        public DeviceGrain(BillingDbContext dbContext, IPaymentSystem paymentSystem, ILogger<DeviceGrain> logger)
        {
            _dbContext = dbContext;
            _paymentSystem = paymentSystem;
            _logger = logger;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _inputStates = await _dbContext.Accounts
                .Where(x => x.DeviceSerial == this.GetPrimaryKeyString())
                .Where(x => x.State == AccountState.Ok)
                .SelectMany(x => x.Payments.DefaultIfEmpty(), (ac, p) => new
                {
                    AccountId = ac.Id,
                    ac.Wallet,
                    ac.DeviceSerial,
                    ac.InputCode,
                    AssetId = ac.Asset.Id,
                    AssetCode = ac.Asset.Code,
                    AssetIssuer = ac.Asset.Issuer,
                    Amount = p == null ? 0M : p.Amount
                })
                .GroupBy(x => x.InputCode)
                .ToDictionaryAsync(x => x.Key, x => new InputInfo
                {
                    AccountId = x.First().AccountId,
                    Wallet = x.First().Wallet,
                    DeviceSerial = x.First().DeviceSerial,
                    AssetId = x.First().AssetId,
                    AssetCode = x.First().AssetCode,
                    AssetIssuer = x.First().AssetIssuer,
                    CurrentValue = x.Sum(p => p.Amount),
                });
        }

        public async Task<MakePaymentReply> MakePaymentAsync(MakePaymentCommand command)
        {
            if (!_inputStates.TryGetValue(command.InputCode, out var inputInfo))
            {
                _logger.LogWarning("Account not found for {InputCode}", command.InputCode);
                return MakePaymentReply.Skipped;
            }

            var incomingValue = (decimal)command.IncomingValue;
            var amount = Math.Round(incomingValue - inputInfo.CurrentValue, 7);

            await _paymentSystem.AddPaymentAsync(new AddPaymentCommand
            {
                RecieverAccountId = inputInfo.Wallet,
                AssetCode = inputInfo.AssetCode,
                AssetIssuerAccountId = inputInfo.AssetIssuer,
                Amount = amount,
            });

            var payment = new Payment
            {
                AccountId = inputInfo.AccountId,
                AssetId = inputInfo.AssetId,
                Amount = amount,
                Status = PaymentStatus.Completed,
            };

            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();

            inputInfo.CurrentValue = incomingValue;

            return new MakePaymentReply { };
        }
    }
}
