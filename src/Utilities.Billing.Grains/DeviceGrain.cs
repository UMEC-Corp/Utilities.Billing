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
        private Dictionary<string, InputInfo> _inputStates;

        public DeviceGrain(BillingDbContext dbContext, IPaymentSystem paymentSystem, ILogger<DeviceGrain> logger)
        {
            _dbContext = dbContext;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _inputStates = _dbContext.Accounts
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
                .ToDictionary(x => x.Key, x => new InputInfo
                {
                    AccountId = x.First().AccountId,
                    Wallet = x.First().Wallet,
                    DeviceSerial = x.First().DeviceSerial,
                    AssetId = x.First().AssetId,
                    AssetCode = x.First().AssetCode,
                    AssetIssuer = x.First().AssetIssuer,
                    CurrentValue = x.Sum(p => p.Amount),
                });

            return Task.CompletedTask;
        }

        public Task<InputInfo?> GetInputState(string code)
        {
            _inputStates.TryGetValue(code, out var inputInfo);
            return Task.FromResult(inputInfo);
        }

        public Task UpdateInputState(string code, InputInfo info)
        {
            _inputStates[code] = info;

            return Task.CompletedTask;
        }

        public Task DeleteInputState(string inputCode)
        {
            _inputStates.Remove(inputCode);

            return Task.CompletedTask;
        }
    }
}
