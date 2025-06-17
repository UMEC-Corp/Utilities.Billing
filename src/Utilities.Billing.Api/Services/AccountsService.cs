using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.Grains;

namespace Utilities.Billing.Api.Services;

/// <summary>
/// Provides account-related operations, including making payments and updating device/account state.
/// </summary>
public class AccountsService : IAccountsService
{
    private readonly BillingDbContext _dbContext;
    private readonly IPaymentSystem _paymentSystem;
    private readonly IGrainFactory _clusterClient;
    private readonly ILogger<AccountsService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountsService"/> class.
    /// </summary>
    /// <param name="dbContext">The billing database context.</param>
    /// <param name="paymentSystem">The payment system service.</param>
    /// <param name="clusterClient">The Orleans grain factory.</param>
    /// <param name="logger">The logger instance.</param>
    public AccountsService(BillingDbContext dbContext, IPaymentSystem paymentSystem, IGrainFactory clusterClient, ILogger<AccountsService> logger)
    {
        _dbContext = dbContext;
        _paymentSystem = paymentSystem;
        _clusterClient = clusterClient;
        _logger = logger;
    }

    /// <summary>
    /// Makes a payment for a device input and updates the account and device state accordingly.
    /// </summary>
    /// <param name="command">The command containing payment details, including device serial, input code, and incoming value.</param>
    /// <returns>A <see cref="MakePaymentReply"/> indicating the result of the operation.</returns>
    /// <remarks>
    /// <para>
    /// This method performs the following steps:
    /// <list type="number">
    ///   <item>
    ///     <description>Retrieves the device grain using the provided device serial and fetches the input state for the specified input code.</description>
    ///   </item>
    ///   <item>
    ///     <description>If the input state is not found, logs the event and returns a skipped reply.</description>
    ///   </item>
    ///   <item>
    ///     <description>Calculates the payment amount as the difference between the incoming value and the current value, rounded to 7 decimal places.</description>
    ///   </item>
    ///   <item>
    ///     <description>Creates a payment on the payment system for the calculated amount and asset details from the input state.</description>
    ///   </item>
    ///   <item>
    ///     <description>Creates a new <see cref="Payment"/> entity, marks it as completed, and saves it to the database.</description>
    ///   </item>
    ///   <item>
    ///     <description>Updates the input state with the new current value and persists it to the device grain.</description>
    ///   </item>
    ///   <item>
    ///     <description>Returns a reply indicating the payment was processed.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
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

        return new MakePaymentReply();
    }
}
