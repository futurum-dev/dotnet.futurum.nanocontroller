using FluentAssertions;

using FluentValidation;

using Futurum.Core.Result;
using Futurum.NanoController.Middleware;
using Futurum.Test.Result;

using Moq;
using Moq.AutoMock;

using Xunit;

namespace Futurum.NanoController.Tests;

public class NanoControllerDispatcherTests
{
    public class NoOpValidator : AbstractValidator<Request>
    {
    }

    public record Request : INanoControllerRequest<Response>;

    public record Response;

    [Fact]
    public async Task Handler_success()
    {
        var handler = new Handler(true);

        var mocker = new AutoMocker();
        mocker.Use<INanoControllerRequestValidation<Request>>(new NanoControllerRequestValidation<Request>(new[] { new NoOpValidator() }));
        mocker.Use<INanoControllerMiddlewareExecutor<Request, Response>>(new DisabledNanoControllerMiddlewareExecutor<Request, Response>());
        mocker.Use<INanoControllerHandler<Request, Response>>(handler);

        var loggerMock = mocker.GetMock<INanoControllerLogger>();

        var nanoControllerDispatcher = mocker.CreateInstance<NanoControllerDispatcher<Request, Response>>();

        var request = new Request();

        var result = await nanoControllerDispatcher.ExecuteAsync(request);

        result.ShouldBeSuccess();

        handler.WasCalled.Should().BeTrue();

        loggerMock.Verify(x => x.RequestReceived<Request, Response>(request), Times.Once);
        loggerMock.Verify(x => x.ResponseSent<Request, Response>(It.IsAny<Response>()), Times.Once);
    }

    [Fact]
    public async Task Handler_failure()
    {
        var handler = new Handler(false);

        var mocker = new AutoMocker();
        mocker.Use<INanoControllerRequestValidation<Request>>(new NanoControllerRequestValidation<Request>(new[] { new NoOpValidator() }));
        mocker.Use<INanoControllerMiddlewareExecutor<Request, Response>>(new DisabledNanoControllerMiddlewareExecutor<Request, Response>());
        mocker.Use<INanoControllerHandler<Request, Response>>(handler);

        var loggerMock = mocker.GetMock<INanoControllerLogger>();

        var nanoControllerDispatcher = mocker.CreateInstance<NanoControllerDispatcher<Request, Response>>();

        var request = new Request();

        var result = await nanoControllerDispatcher.ExecuteAsync(request);

        result.ShouldBeFailureWithError(Handler.ErrorMessage);

        handler.WasCalled.Should().BeTrue();

        loggerMock.Verify(x => x.RequestReceived<Request, Response>(request), Times.Once);
        loggerMock.Verify(x => x.ResponseSent<Request, Response>(It.IsAny<Response>()), Times.Never);
    }

    private class Handler : INanoControllerHandler<Request, Response>
    {
        private readonly bool _isSuccess;

        public const string ErrorMessage = "ERROR-MESSAGE";

        public Handler(bool isSuccess)
        {
            _isSuccess = isSuccess;
        }

        public bool WasCalled { get; private set; }

        public Task<Result<Response>> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            WasCalled = true;

            return _isSuccess
                ? new Response().ToResultOkAsync()
                : Result.FailAsync<Response>(ErrorMessage);
        }
    }
}