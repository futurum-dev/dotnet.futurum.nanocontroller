using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

public interface INanoControllerPostProcessorMiddleware<in TRequest, in TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    Task<Result> ExecuteAsync(TRequest request, TResponse response);
}