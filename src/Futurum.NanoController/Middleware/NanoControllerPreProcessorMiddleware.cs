using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

internal class NanoControllerPreProcessorMiddleware<TRequest, TResponse> : INanoControllerMiddleware<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    private readonly INanoControllerPreProcessorMiddleware<TRequest>[] _middleware;

    public NanoControllerPreProcessorMiddleware(IEnumerable<INanoControllerPreProcessorMiddleware<TRequest>> middleware)
    {
        _middleware = middleware.ToArray();
    }

    public async Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> next)
    {
        async Task<Result> ExecuteMiddleware(INanoControllerPreProcessorMiddleware<TRequest> middleware) =>
            await middleware.ExecuteAsync(request);

        if (_middleware.Length == 0)
            return await next(request);

        return await _middleware.FlatMapSequentialUntilFailureAsync(ExecuteMiddleware)
                                .ThenAsync(async () => await next(request));
    }
}