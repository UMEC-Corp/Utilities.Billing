using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.Grains;

namespace Utilities.Billing.Api.Services;

public class AccountsService : IAccountsService
{
    private readonly BillingDbContext _dbContext;
    private readonly IPaymentSystem _paymentSystem;
    private readonly IGrainFactory _clusterClient;
    private readonly ILogger<AccountsService> _logger;

    public AccountsService(BillingDbContext dbContext, IPaymentSystem paymentSystem, IGrainFactory clusterClient, ILogger<AccountsService> logger)
    {
        _dbContext = dbContext;
        _paymentSystem = paymentSystem;
        _clusterClient = clusterClient;
        _logger = logger;
    }

    public async Task<MakePaymentReply> MakePaymentAsync(MakePaymentCommand command)
    {
        var deviceGrain = _clusterClient.GetGrain<IDeviceGrain>(command.DeviceSerial);
        var inputInfo = await deviceGrain.GetInputState(command.InputCode);
        if (inputInfo == null)
        {
            _logger.LogInformation("Account not found for {InputCode}", command.InputCode);
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
        await deviceGrain.UpdateInputState(command.InputCode, inputInfo);

        return new MakePaymentReply { };
    }
}
