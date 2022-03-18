using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Futurum.NanoController.Tests;

public class WeÍbApiResultErrorTests
{
    [Fact]
    public void GetErrorString()
    {
        var httpStatusCode = HttpStatusCode.BadRequest;
        var detailErrorMessage = Guid.NewGuid().ToString();

        var nanoControllerResultError = new WebApiResultError(httpStatusCode, detailErrorMessage);

        var errorString = nanoControllerResultError.GetErrorString(";");

        errorString.Should().Be($"{ReasonPhrases.GetReasonPhrase((int)httpStatusCode)};{detailErrorMessage}");
    }
    
    [Fact]
    public void GetErrorStructure()
    {
        var httpStatusCode = HttpStatusCode.BadRequest;
        var detailErrorMessage = Guid.NewGuid().ToString();

        var nanoControllerResultError = new WebApiResultError(httpStatusCode, detailErrorMessage);

        var errorStructure = nanoControllerResultError.GetErrorStructure();

        errorStructure.Message.Should().Be(ReasonPhrases.GetReasonPhrase((int)httpStatusCode));
        errorStructure.Children.First().Message.Should().Be(detailErrorMessage);
        errorStructure.Children.First().Children.Should().BeEmpty();
    }
}