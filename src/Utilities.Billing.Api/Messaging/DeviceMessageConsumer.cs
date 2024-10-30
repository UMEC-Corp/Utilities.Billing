using MassTransit;
using Orleans;
using Utilities.Billing.Contracts;
using Utilities.Common.Messages;

namespace Utilities.Billing.Api.Messaging;

public class DeviceMessageConsumer : IConsumer<Batch<DeviceEvent>>
{
    private readonly IGrainFactory _clusterClient;
    private readonly ILogger<DeviceMessageConsumer> _logger;

    public DeviceMessageConsumer(ILogger<DeviceMessageConsumer> logger, IGrainFactory clusterClient)
    {
        _clusterClient = clusterClient;
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
                var account = _clusterClient.GetGrain<IDeviceGrain>(groupByDeviceSerial.Key);

                var inputMessages =
                    groupByDeviceSerial
                        .SelectMany(x =>
                            x.Values.Select(v => new { x.Time, InputCode = v.Key, InputValue = v.Value.Value }))
                        .GroupBy(x => x.InputCode);

                foreach (var message in inputMessages)
                {
                    var (code, value) = message.OrderBy(x => x.Time).Select(x => (x.InputCode, x.InputValue)).Last();

                    await account.MakePaymentAsync(new MakePaymentCommand
                    {
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
