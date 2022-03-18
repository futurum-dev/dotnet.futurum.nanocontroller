using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

internal class NanoControllerPostProcessorMiddleware<TRequest, TResponse> : INanoControllerMiddleware<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    private readonly INanoControllerPostProcessorMiddleware<TRequest, TResponse>[] _middleware;

    public NanoControllerPostProcessorMiddleware(IEnumerable<INanoControllerPostProcessorMiddleware<TRequest, TResponse>> middleware)
    {
        _middleware = middleware.ToArray();
    }

    public async Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> next)
    {
        async Task<Result<TResponse>> ExecuteNext() =>
            await next(request);

        async Task<Result> ExecuteMiddleware(INanoControllerPostProcessorMiddleware<TRequest, TResponse> middleware, TResponse response) =>
            await middleware.ExecuteAsync(request, response);

        if (_middleware.Length == 0)
            return await next(request);

        return await ExecuteNext()
            .ThenAsync(response => _middleware.FlatMapSequentialUntilFailureAsync(middleware => ExecuteMiddleware(middleware, response)));
    }
}