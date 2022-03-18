using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

public interface INanoControllerMiddleware<TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> next);
}