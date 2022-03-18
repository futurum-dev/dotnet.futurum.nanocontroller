using Futurum.Core.Result;
using Futurum.NanoController.Middleware;

namespace Futurum.NanoController;

internal interface INanoControllerDispatcher<TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(INanoControllerRequest<TResponse> request, CancellationToken cancellationToken = default);
}

internal interface INanoControllerDispatcher<in TRequest, TResponse> : INanoControllerDispatcher<TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
}

internal class NanoControllerDispatcher<TRequest, TResponse> : INanoControllerDispatcher<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    private readonly INanoControllerLogger _logger;
    private readonly INanoControllerRequestValidation<TRequest> _requestValidation;
    private readonly INanoControllerMiddlewareExecutor<TRequest, TResponse> _middlewareExecutor;
    private readonly INanoControllerHandler<TRequest, TResponse> _handler;

    public NanoControllerDispatcher(INanoControllerLogger logger,
                                    INanoControllerRequestValidation<TRequest> requestValidation,
                                    INanoControllerMiddlewareExecutor<TRequest, TResponse> middlewareExecutor,
                                    INanoControllerHandler<TRequest, TResponse> handler)
    {
        _logger = logger;
        _requestValidation = requestValidation;
        _middlewareExecutor = middlewareExecutor;
        _handler = handler;
    }

    public Task<Result<TResponse>> ExecuteAsync(INanoControllerRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestTyped = (TRequest)request;

        _logger.RequestReceived<TRequest, TResponse>(requestTyped);

        return _requestValidation.ExecuteAsync(requestTyped)
                                 .ThenAsync(() => _middlewareExecutor.ExecuteAsync(requestTyped, r => ExecuteHandler(r, cancellationToken)))
                                 .DoAsync(response => _logger.ResponseSent<TRequest, TResponse>(response));
    }

    private Task<Result<TResponse>> ExecuteHandler(TRequest request, CancellationToken cancellationToken = default) =>
        _handler.ExecuteAsync(request, cancellationToken);
}