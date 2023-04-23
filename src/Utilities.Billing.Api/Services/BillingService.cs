using Grpc.Core;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Services;

public class BillingService : Protos.BillingService.BillingServiceBase
{
    //public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    //{
    //    return Task.FromResult(new HelloReply() { Message = $"Hello, {request.Name}!" });
    //}
}