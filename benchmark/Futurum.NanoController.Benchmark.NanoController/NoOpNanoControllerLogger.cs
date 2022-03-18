using Futurum.Core.Result;

namespace Futurum.NanoController.Benchmark.NanoController;

public class NoOpNanoControllerLogger : INanoControllerLogger
{
    public void RequestReceived<TRequest, TResponse>(TRequest request)
        where TRequest : INanoControllerRequest<TResponse>
    {
    }

    public void ResponseSent<TRequest, TResponse>(TResponse response)
        where TRequest : INanoControllerRequest<TResponse>
    {
    }

    public void Error<TRequest, TResponse>(TRequest request, IResultError error)
        where TRequest : INanoControllerRequest<TResponse>
    {
    }

    public void UnhandledError<TRequest, TResponse>(TRequest request, Exception exception)
        where TRequest : INanoControllerRequest<TResponse>
    {
    }
}