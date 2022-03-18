using FluentAssertions;

using Futurum.Core.Result;
using Futurum.NanoController.Middleware;
using Futurum.Test.Result;

using Xunit;
using Xunit.Abstractions;

namespace Futurum.NanoController.Tests.Middleware;

public class NanoControllerMiddlewareExecutorTests
{
    private readonly ITestOutputHelper _output;

    private const string ErrorMessage = "ERROR_MESSAGE";

    public NanoControllerMiddlewareExecutorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public record Command : INanoControllerRequest<Response>;

    public record Response;

    private class SuccessHandler : INanoControllerHandler<Command, Response>
    {
        private readonly Action _action;

        public SuccessHandler(Action action)
        {
            _action = action;
        }

        public Task<Result<Response>> ExecuteAsync(Command request, CancellationToken cancellationToken)
        {
            _action();

            return new Response().ToResultOkAsync();
        }
    }

    private class FailureHandler : INanoControllerHandler<Command, Response>
    {
        private readonly Action _action;

        public FailureHandler(Action action)
        {
            _action = action;
        }

        public Task<Result<Response>> ExecuteAsync(Command request, CancellationToken cancellationToken)
        {
            _action();

            return Result.FailAsync<Response>(ErrorMessage);
        }
    }

    public class SuccessMiddleware<TRequest, TResponse> : INanoControllerMiddleware<TRequest, TResponse>
        where TRequest : INanoControllerRequest<TResponse>
    {
        private readonly Action _action;

        public SuccessMiddleware(Action action)
        {
            _action = action;
        }

        public Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> next)
        {
            _action();

            return next(request);
        }
    }

    public class FailureMiddleware<TRequest, TResponse> : INanoControllerMiddleware<TRequest, TResponse>
        where TRequest : INanoControllerRequest<TResponse>
    {
        private readonly Action _action;

        public FailureMiddleware(Action action)
        {
            _action = action;
        }

        public Task<Result<TResponse>> ExecuteAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> next)
        {
            _action();

            return Result.FailAsync<TResponse>(ErrorMessage);
        }
    }

    [Fact]
    public async Task when_there_is_no_middleware_then_the_Handler_is_still_called()
    {
        var handlerWasCalled = false;

        var handler = new SuccessHandler(() =>
        {
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middlewares = new INanoControllerMiddleware<Command, Response>[] { };
        var middlewareExecutor = new NanoControllerMiddlewareExecutor<Command, Response>(middlewares);

        var command = new Command();

        var result = await middlewareExecutor.ExecuteAsync(command, c => handler.ExecuteAsync(c, CancellationToken.None));

        result.ShouldBeSuccess();
        handlerWasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task called_in_the_correct_order()
    {
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;
        var handlerWasCalled = false;

        var testMiddleware1 = new SuccessMiddleware<Command, Response>(() =>
        {
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<Command, Response>(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        var handler = new SuccessHandler(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeTrue();
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middlewares = new[] { testMiddleware1, testMiddleware2 };
        var middlewareExecutor = new NanoControllerMiddlewareExecutor<Command, Response>(middlewares);

        var command = new Command();

        var result = await middlewareExecutor.ExecuteAsync(command, c => handler.ExecuteAsync(c, CancellationToken.None));

        result.ShouldBeSuccess();
        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeTrue();
        handlerWasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task when_Handler_returns_failure_then_correct_error_returned()
    {
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;
        var handlerWasCalled = false;

        var testMiddleware1 = new SuccessMiddleware<Command, Response>(() =>
        {
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<Command, Response>(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        var handler = new FailureHandler(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeTrue();
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middlewares = new INanoControllerMiddleware<Command, Response>[] { testMiddleware1, testMiddleware2 };
        var middlewareExecutor = new NanoControllerMiddlewareExecutor<Command, Response>(middlewares);

        var command = new Command();

        var result = await middlewareExecutor.ExecuteAsync(command, c => handler.ExecuteAsync(c, CancellationToken.None));

        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeTrue();
        handlerWasCalled.Should().BeTrue();

        result.ShouldBeFailureWithError(ErrorMessage);
    }

    [Fact]
    public async Task when_middleware_returns_failure_then_no_more_called_and_correct_error_returned()
    {
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;
        var handlerWasCalled = false;

        var testMiddleware1 = new FailureMiddleware<Command, Response>(() =>
        {
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<Command, Response>(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        var handler = new FailureHandler(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeTrue();
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middlewares = new INanoControllerMiddleware<Command, Response>[] { testMiddleware1, testMiddleware2 };
        var middlewareExecutor = new NanoControllerMiddlewareExecutor<Command, Response>(middlewares);

        var command = new Command();

        var result = await middlewareExecutor.ExecuteAsync(command, c => handler.ExecuteAsync(c, CancellationToken.None));

        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeFalse();
        handlerWasCalled.Should().BeFalse();

        result.ShouldBeFailureWithError(ErrorMessage);
    }
}