using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

internal class DisabledNanoControllerMiddlewareExecutor<TRequest, TResponse> : INanoControllerMiddlewareExecutor<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    public Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> handlerFunc) =>
        handlerFunc(request);
}