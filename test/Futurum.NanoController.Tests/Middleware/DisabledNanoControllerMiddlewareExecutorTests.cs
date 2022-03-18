using FluentAssertions;

using Futurum.Core.Result;
using Futurum.NanoController.Middleware;
using Futurum.Test.Result;

using Xunit;
using Xunit.Abstractions;

namespace Futurum.NanoController.Tests.Middleware;

public class DisabledNanoControllerMiddlewareExecutorTests
{
    private readonly ITestOutputHelper _output;

    public DisabledNanoControllerMiddlewareExecutorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public record Command : INanoControllerRequest<Response>;

    public record Response;

    private class Handler : INanoControllerHandler<Command, Response>
    {
        private readonly bool _isSuccess;

        public const string ErrorMessage = "ERROR-MESSAGE";

        public Handler(bool isSuccess)
        {
            _isSuccess = isSuccess;
        }

        public bool WasCalled { get; private set; }

        public Task<Result<Response>> ExecuteAsync(Command request, CancellationToken cancellationToken)
        {
            WasCalled = true;

            return _isSuccess
                ? new Response().ToResultOkAsync()
                : Result.FailAsync<Response>(ErrorMessage);
        }
    }

    [Fact]
    public async Task verify_calls_Handler_and_returns_correctly_for_success()
    {
        var middlewareExecutor = new DisabledNanoControllerMiddlewareExecutor<Command, Response>();

        var handler = new Handler(true);

        var request = new Command();

        var result = await middlewareExecutor.ExecuteAsync(request, c => handler.ExecuteAsync(c, CancellationToken.None));

        result.ShouldBeSuccessWithValue(new Response());

        handler.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task verify_calls_Handler_and_returns_correctly_for_failure()
    {
        var middlewareExecutor = new DisabledNanoControllerMiddlewareExecutor<Command, Response>();

        var handler = new Handler(false);

        var request = new Command();

        var result = await middlewareExecutor.ExecuteAsync(request, c => handler.ExecuteAsync(c, CancellationToken.None));

        result.ShouldBeFailureWithError(Handler.ErrorMessage);

        handler.WasCalled.Should().BeTrue();
    }
}