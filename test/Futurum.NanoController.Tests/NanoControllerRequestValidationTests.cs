using FluentValidation;

using Futurum.Core.Result;
using Futurum.Test.Result;

using Xunit;
using Xunit.Abstractions;

namespace Futurum.NanoController.Tests;

public class NanoControllerRequestValidationTests
{
    private readonly ITestOutputHelper _output;

    public NanoControllerRequestValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public class NoValidation
    {
        private record Request;

        [Fact]
        public async Task success()
        {
            var result = await TestRunner();

            result.ShouldBeSuccess();
        }

        private static Task<Result> TestRunner()
        {
            var nanoControllerRequestValidation = new NanoControllerRequestValidation<Request>(Array.Empty<IValidator<Request>>());

            return nanoControllerRequestValidation.ExecuteAsync(new Request());
        }
    }

    public class OneValidation
    {
        private const string PropertyName = "Name";
        private const string ErrorMessage = "ErrorMessage";

        private record Request(string Name);

        [Fact]
        public async Task when_no_validation_errors_then_success()
        {
            var result = await TestRunner(new ValidatorSuccess());

            result.ShouldBeSuccess();
        }

        [Fact]
        public async Task when_validation_errors_then_failure()
        {
            var result = await TestRunner(new ValidatorFailure());

            result.ShouldBeFailureWithError($"Validation failure for '{PropertyName}' with error : '{ErrorMessage}'");
        }

        private static Task<Result> TestRunner(IValidator<Request> validator)
        {
            var nanoControllerRequestValidation = new NanoControllerRequestValidation<Request>(new[] { validator });

            return nanoControllerRequestValidation.ExecuteAsync(new Request(Guid.NewGuid().ToString()));
        }

        private class ValidatorSuccess : AbstractValidator<Request>
        {
        }

        private class ValidatorFailure : AbstractValidator<Request>
        {
            public ValidatorFailure()
            {
                RuleFor(x => x.Name).Empty().WithMessage(ErrorMessage);
            }
        }
    }

    public class MoreValidation
    {
        private const string PropertyName = "Name";
        private const string ErrorMessage = "ErrorMessage";

        private record Request(string Name);

        [Fact]
        public async Task when_no_validation_errors_then_success()
        {
            var result = await TestRunner(new ValidatorSuccess());

            result.ShouldBeSuccess();
        }

        [Fact]
        public async Task when_validation_errors_then_failure()
        {
            var result = await TestRunner(new ValidatorFailure());

            var errorMessage = $"Validation failure for '{PropertyName}' with error : '{ErrorMessage}'";
            result.ShouldBeFailureWithError($"{errorMessage};{errorMessage}");
        }

        private static Task<Result> TestRunner(IValidator<Request> validator)
        {
            var nanoControllerRequestValidation = new NanoControllerRequestValidation<Request>(new[] { validator, validator });

            return nanoControllerRequestValidation.ExecuteAsync(new Request(Guid.NewGuid().ToString()));
        }

        private class ValidatorSuccess : AbstractValidator<Request>
        {
        }

        private class ValidatorFailure : AbstractValidator<Request>
        {
            public ValidatorFailure()
            {
                RuleFor(x => x.Name).Empty().WithMessage(ErrorMessage);
            }
        }
    }
}