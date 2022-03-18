using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

internal interface INanoControllerMiddlewareExecutor<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> handlerFunc);
}