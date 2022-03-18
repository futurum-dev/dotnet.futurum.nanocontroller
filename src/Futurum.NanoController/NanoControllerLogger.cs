using System.Diagnostics.CodeAnalysis;

using Futurum.Core.Result;

using ILogger = Serilog.ILogger;

namespace Futurum.NanoController;

[ExcludeFromCodeCoverage]
public class NanoControllerLogger : INanoControllerLogger
{
    private readonly ILogger _logger;

    public NanoControllerLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void RequestReceived<TRequest, TResponse>(TRequest request)
        where TRequest : INanoControllerRequest<TResponse>
    {
        var eventData = new RequestReceivedData<TRequest>(typeof(TRequest), typeof(TResponse), request);

        _logger.Debug("NanoController request received {@eventData}", eventData);
    }

    public void ResponseSent<TRequest, TResponse>(TResponse response)
        where TRequest : INanoControllerRequest<TResponse>
    {
        var eventData = new ResponseSentData<TResponse>(typeof(TRequest), typeof(TResponse), response);

        _logger.Debug("NanoController response sent {@eventData}", eventData);
    }

    public void Error<TRequest, TResponse>(TRequest request, IResultError error)
        where TRequest : INanoControllerRequest<TResponse>
    {
        var eventData = new ErrorData<TRequest>(request, error.ToErrorString());

        _logger.Error("NanoController error {@eventData}", eventData);
    }

    public void UnhandledError<TRequest, TResponse>(TRequest request, Exception exception)
        where TRequest : INanoControllerRequest<TResponse>
    {
        var eventData = new UnhandledErrorData<TRequest>(request);

        _logger.Error(exception, "NanoController unhandled error {@eventData}", eventData);
    }

    private record struct RequestReceivedData<TRequest>(Type RequestType, Type ResponseType, TRequest Request);

    private record struct ResponseSentData<TResponse>(Type RequestType, Type ResponseType, TResponse Response);

    private record struct ErrorData<TRequest>(TRequest Request, string Error);

    private record struct UnhandledErrorData<TRequest>(TRequest Request);
}