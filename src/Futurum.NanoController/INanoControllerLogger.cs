using Futurum.Core.Result;

namespace Futurum.NanoController;

public interface INanoControllerLogger
{
    void RequestReceived<TRequest, TResponse>(TRequest request)
        where TRequest : INanoControllerRequest<TResponse>;

    void ResponseSent<TRequest, TResponse>(TResponse response)
        where TRequest : INanoControllerRequest<TResponse>;

    void Error<TRequest, TResponse>(TRequest request, IResultError error)
        where TRequest : INanoControllerRequest<TResponse>;

    void UnhandledError<TRequest, TResponse>(TRequest request, Exception exception)
        where TRequest : INanoControllerRequest<TResponse>;
}