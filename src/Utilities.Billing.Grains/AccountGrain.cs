using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Utilities.Billing.Grains
{
    public class DeviceGrain : Grain, IDeviceGrain
    {
        private readonly BillingDbContext _dbContext;
        private readonly IPaymentSystem _paymentSystem;
        private Dictionary<string, InputInfo> _inputStates;

        public DeviceGrain(BillingDbContext dbContext, IPaymentSystem paymentSystem)
        {
            _dbContext = dbContext;
            _paymentSystem = paymentSystem;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var deviceSerial = this.GetPrimaryKeyString();

            var query = _dbContext.Accounts.Where(x => string.Compare(x.DeviceSerial, deviceSerial, true) == 0);

            _inputStates = await _dbContext.Accounts
                .Where(x => string.Compare(x.DeviceSerial, deviceSerial, true) == 0)
                .SelectMany(x => x.Payments, (ac, p) => new
                {
                    AccountId = ac.Id,
                    ac.Wallet,
                    ac.DeviceSerial,
                    ac.InputCode,
                    AssetId = ac.Asset.Id,
                    AssetCode = ac.Asset.Code,
                    AssetIssuer = ac.Asset.Issuer,
                    p.Amount
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

        public async Task<MakePaymentReply> MakePayment(MakePaymentCommand command)
        {
            if (!_inputStates.TryGetValue(command.InputCode, out var inputInfo))
            {
                return null;
            }

            var incomingValue = (decimal)double.Parse(command.IncomingValue);
            var amount = incomingValue - inputInfo.CurrentValue;

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
