using MassTransit;
using Orleans;
using Utilities.Billing.Api.Services;
using Utilities.Billing.Contracts;
using Utilities.Billing.Grains;
using Utilities.Common.Messages;

namespace Utilities.Billing.Api.Messaging;

public class DeviceMessageConsumer : IConsumer<Batch<DeviceEvent>>
{
    private readonly IGrainFactory _clusterClient;
    private readonly IAccountsService _accountsService;
    private readonly ILogger<DeviceMessageConsumer> _logger;

    public DeviceMessageConsumer(ILogger<DeviceMessageConsumer> logger, IGrainFactory clusterClient, IAccountsService accountsService)
    {
        _clusterClient = clusterClient;
        _accountsService = accountsService;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<Batch<DeviceEvent>> context)
    {
        try
        {
            var messages = context.Message
                .Where(x => x.Message.EventName == "sensor_data")
                .Select(x => x.Message).ToArray();

            if (!messages.Any())
            {
                return;
            }

            foreach (var groupByDeviceSerial in messages.GroupBy(x => x.DeviceSerial))
            {
                using var _ = _logger.BeginScope("DeviceSerial: {DeviceSerial}", groupByDeviceSerial.Key);

                var inputMessages = groupByDeviceSerial
                        .SelectMany(x =>
                            x.Values.Select(v => new { x.Time, InputCode = v.Key, InputValue = v.Value.Value }))
                        .GroupBy(x => x.InputCode);

                foreach (var message in inputMessages)
                {
                    var (code, value) = message.OrderBy(x => x.Time).Select(x => (x.InputCode, x.InputValue)).Last();

                    await _accountsService.MakePaymentAsync(new MakePaymentCommand
                    {
                        DeviceSerial = groupByDeviceSerial.Key,
                        InputCode = code,
                        IncomingValue = value
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to make payments from device messages");
            throw;
        }
    }
}
