using Futurum.Core.Result;

namespace Futurum.NanoController.Middleware;

public interface INanoControllerPreProcessorMiddleware<in TRequest>
{
    Task<Result> ExecuteAsync(TRequest request);
}