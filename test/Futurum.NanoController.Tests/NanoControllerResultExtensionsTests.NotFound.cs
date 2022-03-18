using System.Net;

using FluentAssertions;

using Futurum.Core.Result;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Futurum.NanoController.Tests;

public class NanoControllerResultExtensionsNotFoundTests
{
    private const string ErrorMessage = "ERROR_MESSAGE_1";
    private const string RequestPath = "/REQUEST_PATH";

    public class TestNanoController : NanoController.Base
    {
        public TestNanoController() : base(null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Path = RequestPath}
                }
            };
        }
    }

    public class Sync
    {
        public class NonGeneric
        {
            [Fact]
            public void Failure()
            {
                var result = Result.Fail(ErrorMessage);

                var actionResult = result.ToNotFound(new TestNanoController());

                actionResult.Should().BeOfType<ObjectResult>();

                var objectResult = actionResult as ObjectResult;

                var problemDetails = objectResult.Value as ProblemDetails;

                problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.NotFound));
                problemDetails.Detail.Should().Be(ErrorMessage);
                problemDetails.Status.Should().Be((int)HttpStatusCode.NotFound);
                problemDetails.Instance.Should().Be(RequestPath);
            }

            [Fact]
            public void Success()
            {
                var result = Result.Ok();

                var actionResult = result.ToNotFound(new TestNanoController());

                actionResult.Should().BeOfType<OkResult>();
            }
        }

        public class Generic
        {
            [Fact]
            public void Failure()
            {
                var result = Result.Fail<int>(ErrorMessage);

                var actionResult = result.ToNotFound(new TestNanoController());

                actionResult.Should().BeOfType<ActionResult<int>>();

                var objectResult = actionResult.Result as ObjectResult;

                var problemDetails = objectResult.Value as ProblemDetails;

                problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.NotFound));
                problemDetails.Detail.Should().Be(ErrorMessage);
                problemDetails.Status.Should().Be((int)HttpStatusCode.NotFound);
                problemDetails.Instance.Should().Be(RequestPath);
            }

            [Fact]
            public void Success()
            {
                var value = 1;

                var result = Result.Ok(value);

                var actionResult = result.ToNotFound(new TestNanoController());

                var okActionResult = actionResult.Result as OkObjectResult;

                var actionResultValue = okActionResult.Value;

                actionResultValue.Should().Be(value);
            }
        }
    }

    public class Async
    {
        public class NonGeneric
        {
            [Fact]
            public async Task Failure()
            {
                var result = Result.FailAsync(ErrorMessage);

                var actionResult = await result.ToNotFoundAsync(new TestNanoController());

                actionResult.Should().BeOfType<ObjectResult>();

                var objectResult = actionResult as ObjectResult;

                var problemDetails = objectResult.Value as ProblemDetails;

                problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.NotFound));
                problemDetails.Detail.Should().Be(ErrorMessage);
                problemDetails.Status.Should().Be((int)HttpStatusCode.NotFound);
                problemDetails.Instance.Should().Be(RequestPath);
            }

            [Fact]
            public async Task Success()
            {
                var result = Result.OkAsync();

                var actionResult = await result.ToNotFoundAsync(new TestNanoController());

                actionResult.Should().BeOfType<OkResult>();
            }
        }

        public class Generic
        {
            [Fact]
            public async Task Failure()
            {
                var result = Result.FailAsync<int>(ErrorMessage);

                var actionResult = await result.ToNotFoundAsync(new TestNanoController());

                actionResult.Should().BeOfType<ActionResult<int>>();

                var objectResult = actionResult.Result as ObjectResult;

                var problemDetails = objectResult.Value as ProblemDetails;

                problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.NotFound));
                problemDetails.Detail.Should().Be(ErrorMessage);
                problemDetails.Status.Should().Be((int)HttpStatusCode.NotFound);
                problemDetails.Instance.Should().Be(RequestPath);
            }

            [Fact]
            public async Task Success()
            {
                var value = 1;

                var result = Result.OkAsync(value);

                var actionResult = await result.ToNotFoundAsync(new TestNanoController());

                var okActionResult = actionResult.Result as OkObjectResult;

                var actionResultValue = okActionResult.Value;

                actionResultValue.Should().Be(value);
            }
        }

        public class Unit
        {
            [Fact]
            public async Task Failure()
            {
                var result = Result.FailAsync<Futurum.Core.Functional.Unit>(ErrorMessage);

                var actionResult = await result.ToNotFoundAsync(new TestNanoController());

                actionResult.Should().BeOfType<ObjectResult>();

                var objectResult = actionResult as ObjectResult;

                var problemDetails = objectResult.Value as ProblemDetails;

                problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.NotFound));
                problemDetails.Detail.Should().Be(ErrorMessage);
                problemDetails.Status.Should().Be((int)HttpStatusCode.NotFound);
                problemDetails.Instance.Should().Be(RequestPath);
            }

            [Fact]
            public async Task Success()
            {
                var result = Result.OkAsync(Futurum.Core.Functional.Unit.Value);

                var actionResult = await result.ToNotFoundAsync(new TestNanoController());

                actionResult.Should().BeOfType<OkResult>();
            }
        }
    }
}