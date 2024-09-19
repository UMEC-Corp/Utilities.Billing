using MassTransit;
using Orleans;
using Utilities.Billing.Contracts;
using Utilities.Common.Messages;

namespace Utilities.Billing.Api.Messaging;

public class DeviceMessageConsumer : IConsumer<Batch<DeviceMessageReceived>>
{
    private readonly IGrainFactory _clusterClient;
    private readonly ILogger<DeviceMessageConsumer> _logger;

    public DeviceMessageConsumer(ILogger<DeviceMessageConsumer> logger, IGrainFactory clusterClient)
    {
        _clusterClient = clusterClient;
        _logger = logger;
    }
    public Task Consume(ConsumeContext<Batch<DeviceMessageReceived>> context)
    {
        try
        {
            var messages = context.Message
                .Where(x => x.Message.Subject == "input")
                .Select(x => x.Message).ToArray();

            if (!messages.Any())
            {
                return Task.CompletedTask;
            }

            foreach (var groupByDeviceId in messages.GroupBy(x => x.DeviceSerial))
            {
                var account = _clusterClient.GetGrain<IDeviceGrain>(groupByDeviceId.Key);

                foreach (var message in groupByDeviceId.OrderBy(x => x.Timestamp))
                {
                    account.MakePayment(new MakePaymentCommand
                    {
                        InputCode = message.Item,
                        IncomingValue = message.Payload
                    });
                }
            }

            return Task.CompletedTask; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to make payments from device messages");
            throw;
        }
    }
}
