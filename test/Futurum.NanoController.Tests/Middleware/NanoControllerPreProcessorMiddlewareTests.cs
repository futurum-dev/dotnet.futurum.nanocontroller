using FluentAssertions;

using Futurum.Core.Result;
using Futurum.NanoController.Middleware;
using Futurum.Test.Result;

using Xunit;

namespace Futurum.NanoController.Tests.Middleware;

public class NanoControllerPreProcessorMiddlewareTests
{
    private const string ErrorMessage = "ERROR_MESSAGE_1";

    public class SuccessRequest : INanoControllerRequest<SuccessResponse>
    {
    }

    public class SuccessResponse
    {
    }

    public class SuccessHandler : INanoControllerHandler<SuccessRequest, SuccessResponse>
    {
        private readonly Action _action;

        public SuccessHandler(Action action)
        {
            _action = action;
        }

        public Task<Result<SuccessResponse>> ExecuteAsync(SuccessRequest request, CancellationToken cancellationToken = default)
        {
            _action();

            return Result.OkAsync(new SuccessResponse());
        }
    }

    public class FailureRequest : INanoControllerRequest<FailureResponse>
    {
    }

    public class FailureResponse
    {
    }

    public class FailureHandler : INanoControllerHandler<FailureRequest, FailureResponse>
    {
        private readonly Action _action;

        public FailureHandler(Action action)
        {
            _action = action;
        }

        public Task<Result<FailureResponse>> ExecuteAsync(FailureRequest request, CancellationToken cancellationToken = default)
        {
            _action();

            return Result.FailAsync<FailureResponse>(ErrorMessage);
        }
    }

    public class SuccessMiddleware<TRequest, TResponse> : INanoControllerPreProcessorMiddleware<TRequest>
        where TRequest : INanoControllerRequest<TResponse>
    {
        private readonly Action _action;

        public SuccessMiddleware(Action action)
        {
            _action = action;
        }

        public Task<Result> ExecuteAsync(TRequest request)
        {
            _action();

            return Result.OkAsync();
        }
    }

    public class FailureMiddleware<TRequest, TResponse> : INanoControllerPreProcessorMiddleware<TRequest>
        where TRequest : INanoControllerRequest<TResponse>
    {
        private readonly Action _action;

        public FailureMiddleware(Action action)
        {
            _action = action;
        }

        public Task<Result> ExecuteAsync(TRequest request)
        {
            _action();

            return Result.FailAsync(ErrorMessage);
        }
    }

    [Fact]
    public async Task when_there_is_no_middleware_then_the_HandlerÏ_is_still_called()
    {
        var handlerWasCalled = false;

        var testHandler = new SuccessHandler(() =>
        {
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middlewares = new INanoControllerPreProcessorMiddleware<SuccessRequest>[] { };
        var preProcessorMiddleware = new NanoControllerPreProcessorMiddleware<SuccessRequest, SuccessResponse>(middlewares);

        var testRequest = new SuccessRequest();
        var result = await preProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        result.ShouldBeSuccess();
        
        handlerWasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task called_in_the_correct_order()
    {
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;
        var handlerWasCalled = false;

        var testMiddleware1 = new SuccessMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        var testHandler = new SuccessHandler(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeTrue();
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middleware = new[] { testMiddleware1, testMiddleware2 };
        var preProcessorMiddleware = new NanoControllerPreProcessorMiddleware<SuccessRequest, SuccessResponse>(middleware);

        var testRequest = new SuccessRequest();
        var result = await preProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeTrue();
        handlerWasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task when_handler_returns_failure_then_correct_error_returned()
    {
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;
        var handlerWasCalled = false;

        var testMiddleware1 = new SuccessMiddleware<FailureRequest, FailureResponse>(() =>
        {
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<FailureRequest, FailureResponse>(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        var testHandler = new FailureHandler(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeTrue();
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middleware = new INanoControllerPreProcessorMiddleware<FailureRequest>[] { testMiddleware1, testMiddleware2 };
        var preProcessorMiddleware = new NanoControllerPreProcessorMiddleware<FailureRequest, FailureResponse>(middleware);

        var testRequest = new FailureRequest();
        var result = await preProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

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

        var testMiddleware1 = new FailureMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();
            handlerWasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        var testHandler = new SuccessHandler(() =>
        {
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeTrue();
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middleware = new INanoControllerPreProcessorMiddleware<SuccessRequest>[] { testMiddleware1, testMiddleware2 };
        var preProcessorMiddleware = new NanoControllerPreProcessorMiddleware<SuccessRequest, SuccessResponse>(middleware);

        var testRequest = new SuccessRequest();
        var result = await preProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeFalse();
        handlerWasCalled.Should().BeFalse();

        result.ShouldBeFailureWithError(ErrorMessage);
    }
}