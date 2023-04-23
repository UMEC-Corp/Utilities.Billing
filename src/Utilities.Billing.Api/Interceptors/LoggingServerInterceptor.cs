using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Utilities.Billing.Api.Interceptors;

public class LoggingServerInterceptor : Interceptor
{
    private readonly ILogger<LoggingServerInterceptor> _logger;

    public LoggingServerInterceptor(ILogger<LoggingServerInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        using var _ = _logger.BeginScope(new Dictionary<string, object>
        {
            { "Method", context.Method },
            { "Status", context.Status },
            { "Host", context.Host },
            { "Peer", context.Peer },
            { "Request", request }
        });

        _logger.LogDebug("Starting call. Method: {Method}. Request: {Request}", context.Method, request);

        TResponse? response = default;

        try
        {
            response = await base.UnaryServerHandler(request, context, continuation);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception calling method. Method: {Method}. Request: {Request}", context.Method, request);

            throw;
        }
        finally
        {
            _logger.LogDebug("Finishing call. Method: {Method}. Response: {Response}.", context.Method, response);
        }

        return response;
    }
}
