using FluentAssertions;

using Futurum.Core.Result;
using Futurum.NanoController.Middleware;
using Futurum.Test.Result;

using Xunit;

namespace Futurum.NanoController.Tests.Middleware;

public class NanoControllerPostProcessorMiddlewareTests
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

    public class SuccessMiddleware<TRequest, TResponse> : INanoControllerPostProcessorMiddleware<TRequest, TResponse>
        where TRequest : INanoControllerRequest<TResponse>
    {
        private readonly Action _action;

        public SuccessMiddleware(Action action)
        {
            _action = action;
        }

        public Task<Result> ExecuteAsync(TRequest request, TResponse response)
        {
            _action();

            return Result.OkAsync();
        }
    }

    public class FailureMiddleware<TRequest, TResponse> : INanoControllerPostProcessorMiddleware<TRequest, TResponse>
        where TRequest : INanoControllerRequest<TResponse>
    {
        private readonly Action _action;

        public FailureMiddleware(Action action)
        {
            _action = action;
        }

        public Task<Result> ExecuteAsync(TRequest request, TResponse response)
        {
            _action();

            return Result.FailAsync(ErrorMessage);
        }
    }

    [Fact]
    public async Task when_there_is_no_middleware_then_the_Handler_is_still_called()
    {
        var handlerWasCalled = false;

        var testHandler = new SuccessHandler(() =>
        {
            handlerWasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middlewares = new INanoControllerPostProcessorMiddleware<SuccessRequest, SuccessResponse>[] { };
        var postProcessorMiddleware = new NanoControllerPostProcessorMiddleware<SuccessRequest, SuccessResponse>(middlewares);

        var testRequest = new SuccessRequest();
        var result = await postProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        result.ShouldBeSuccess();
        handlerWasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task called_in_the_correct_order()
    {
        var handlerWasCalled = false;
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;

        var testHandler = new SuccessHandler(() =>
        {
            handlerWasCalled.Should().BeFalse();
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        var testMiddleware1 = new SuccessMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            handlerWasCalled.Should().BeTrue();
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            handlerWasCalled.Should().BeTrue();
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middleware = new[] { testMiddleware1, testMiddleware2 };
        var postProcessorMiddleware = new NanoControllerPostProcessorMiddleware<SuccessRequest, SuccessResponse>(middleware);

        var testRequest = new SuccessRequest();
        var result = await postProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        handlerWasCalled.Should().BeTrue();
        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task when_handler_returns_failure_then_no_middleware_called_and_correct_error_returned_1()
    {
        var handlerWasCalled = false;
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;

        var testHandler = new FailureHandler(() =>
        {
            handlerWasCalled.Should().BeFalse();
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        var testMiddleware1 = new FailureMiddleware<FailureRequest, FailureResponse>(() =>
        {
            handlerWasCalled.Should().BeTrue();
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new FailureMiddleware<FailureRequest, FailureResponse>(() =>
        {
            handlerWasCalled.Should().BeTrue();
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middleware = new INanoControllerPostProcessorMiddleware<FailureRequest, FailureResponse>[] { testMiddleware1, testMiddleware2 };
        var postProcessorMiddleware = new NanoControllerPostProcessorMiddleware<FailureRequest, FailureResponse>(middleware);

        var testRequest = new FailureRequest();
        var result = await postProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        handlerWasCalled.Should().BeTrue();
        middleware1WasCalled.Should().BeFalse();
        middleware2WasCalled.Should().BeFalse();

        result.ShouldBeFailureWithError(ErrorMessage);
    }

    [Fact]
    public async Task when_middleware_returns_failure_then_no_more_called_and_correct_error_returned_1()
    {
        var handlerWasCalled = false;
        var middleware1WasCalled = false;
        var middleware2WasCalled = false;

        var testHandler = new SuccessHandler(() =>
        {
            handlerWasCalled.Should().BeFalse();
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();

            handlerWasCalled = true;
        });

        var testMiddleware1 = new FailureMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            handlerWasCalled.Should().BeTrue();
            middleware1WasCalled.Should().BeFalse();
            middleware2WasCalled.Should().BeFalse();

            middleware1WasCalled = true;
        });

        var testMiddleware2 = new SuccessMiddleware<SuccessRequest, SuccessResponse>(() =>
        {
            handlerWasCalled.Should().BeTrue();
            middleware1WasCalled.Should().BeTrue();
            middleware2WasCalled.Should().BeFalse();

            middleware2WasCalled = true;
        });

        handlerWasCalled.Should().BeFalse();

        var middleware = new INanoControllerPostProcessorMiddleware<SuccessRequest, SuccessResponse>[] { testMiddleware1, testMiddleware2 };
        var postProcessorMiddleware = new NanoControllerPostProcessorMiddleware<SuccessRequest, SuccessResponse>(middleware);

        var testRequest = new SuccessRequest();
        var result = await postProcessorMiddleware.ExecuteAsync(testRequest, r => testHandler.ExecuteAsync(r));

        handlerWasCalled.Should().BeTrue();
        middleware1WasCalled.Should().BeTrue();
        middleware2WasCalled.Should().BeFalse();

        result.ShouldBeFailureWithError(ErrorMessage);
    }
}