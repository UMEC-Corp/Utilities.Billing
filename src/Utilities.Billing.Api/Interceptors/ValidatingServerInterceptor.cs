using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Utilities.Billing.Api.Interceptors;

public class ValidatingServerInterceptor : Interceptor
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ValidatingServerInterceptor> _logger;

    public ValidatingServerInterceptor(IServiceProvider services, ILogger<ValidatingServerInterceptor> logger)
    {
        _services = services;
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var validation = await ValidateRequest(request);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Request {Request} to method {Method} has failed validation: {ValidationErrors}",
                request, context.Method, validation.Errors);

            throw new RpcException(new Grpc.Core.Status(StatusCode.InvalidArgument, "Request validation failed."));
        }

        return await base.UnaryServerHandler(request, context, continuation);
    }

    private Task<ValidationResult> ValidateRequest<TRequest>(TRequest request)
        where TRequest : class
    {
        var validator = _services.GetService<IValidator<TRequest>>();

        return validator != null ? validator.ValidateAsync(request) : Task.FromResult(new ValidationResult());
    }

}
