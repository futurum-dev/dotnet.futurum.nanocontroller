using System.Net;

using FluentAssertions;

using Futurum.Core.Result;
using Futurum.Test.Option;

using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Futurum.NanoController.Tests;

public class WebApiResultErrorExtensionsTests
{
    [Fact]
    public void without_DetailErrorMessage()
    {
        var httpStatusCode = HttpStatusCode.BadRequest;

        var resultError = httpStatusCode.ToResultError();

        resultError.Should().BeOfType<WebApiResultError>();

        var nanoControllerResultError = resultError as WebApiResultError;
        nanoControllerResultError.HttpStatusCode.Should().Be(httpStatusCode);
        nanoControllerResultError.Parent.ShouldBeHasValueWithValue(x => x.GetErrorString(),ReasonPhrases.GetReasonPhrase((int)httpStatusCode));
        nanoControllerResultError.Children.Single().Should().BeOfType<ResultErrorEmpty>();
    }
    
    [Fact]
    public void with_DetailErrorMessage()
    {
        var httpStatusCode = HttpStatusCode.BadRequest;
        var detailErrorMessage = Guid.NewGuid().ToString();

        var resultError = httpStatusCode.ToResultError(detailErrorMessage);

        resultError.Should().BeOfType<WebApiResultError>();

        var nanoControllerResultError = resultError as WebApiResultError;
        nanoControllerResultError.HttpStatusCode.Should().Be(httpStatusCode);
        nanoControllerResultError.Parent.ShouldBeHasValueWithValue(x => x.GetErrorString(),ReasonPhrases.GetReasonPhrase((int)httpStatusCode));
        nanoControllerResultError.Children.Single().ToErrorString().Should().Be(detailErrorMessage);
    }
    
    [Fact]
    public void with_ResultError()
    {
        var httpStatusCode = HttpStatusCode.BadRequest;
        var detailErrorMessage = Guid.NewGuid().ToString();

        var resultError = httpStatusCode.ToResultError(detailErrorMessage.ToResultError());

        resultError.Should().BeOfType<WebApiResultError>();

        var nanoControllerResultError = resultError as WebApiResultError;
        nanoControllerResultError.HttpStatusCode.Should().Be(httpStatusCode);
        nanoControllerResultError.Parent.ShouldBeHasValueWithValue(x => x.GetErrorString(),ReasonPhrases.GetReasonPhrase((int)httpStatusCode));
        nanoControllerResultError.Children.Single().ToErrorString().Should().Be(detailErrorMessage);
    }
}