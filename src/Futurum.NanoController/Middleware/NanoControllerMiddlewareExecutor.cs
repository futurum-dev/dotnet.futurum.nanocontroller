using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

internal class NanoControllerMiddlewareExecutor<TRequest, TResponse> : INanoControllerMiddlewareExecutor<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    private readonly INanoControllerMiddleware<TRequest, TResponse>[] _middleware;

    public NanoControllerMiddlewareExecutor(IEnumerable<INanoControllerMiddleware<TRequest, TResponse>> middleware)
    {
        _middleware = middleware.Reverse().ToArray();
    }

    public Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> handlerFunc)
    {
        if (_middleware.Length == 0)
            return handlerFunc(request);

        return _middleware.Aggregate(handlerFunc, (next, middleware) => r => middleware.ExecuteAsync(r, next))(request);
    }
}