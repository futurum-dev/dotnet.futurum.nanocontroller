using System.Net;

using FluentAssertions;

using FluentValidation.Results;

using Futurum.Core.Result;
using Futurum.FluentValidation;
using Futurum.NanoController.Internal;

using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Futurum.NanoController.Tests.Internal;

public class ResultErrorProblemDetailsExtensionsTests
{
    [Fact]
    public void when_ResultErrorEmpty()
    {
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();

        var resultError = ResultErrorEmpty.Value;

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase(failedStatusCode));
        problemDetails.Detail.Should().BeEmpty();
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be(failedStatusCode);
    }
    
    [Fact]
    public void when_ResultErrorMessage()
    {
        var errorMessage = Guid.NewGuid().ToString();
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();

        var resultError = errorMessage.ToResultError();

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase(failedStatusCode));
        problemDetails.Detail.Should().Be(errorMessage);
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be(failedStatusCode);
    }
    
    [Fact]
    public void when_ResultErrorException()
    {
        var errorMessage = Guid.NewGuid().ToString();
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();

        var resultError = new Exception(errorMessage).ToResultError();

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase(failedStatusCode));
        problemDetails.Detail.Should().Be(errorMessage);
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be(failedStatusCode);
    }
    
    [Fact]
    public void when_ResultErrorComposite()
    {
        var errorMessage1 = Guid.NewGuid().ToString();
        var errorMessage2 = Guid.NewGuid().ToString();
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();

        var resultError1 = errorMessage1.ToResultError();
        var resultError2 = errorMessage2.ToResultError();

        var resultError = ResultErrorCompositeExtensions.ToResultError(resultError1, resultError2);

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase(failedStatusCode));
        problemDetails.Detail.Should().Be(errorMessage1);
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be(failedStatusCode);
    }
    
    [Fact]
    public void when_ResultErrorKeyNotFound()
    {
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();
        
        var httpStatusCode = HttpStatusCode.NotFound;
        var key = Guid.NewGuid().ToString();
        var sourceDescription = Guid.NewGuid().ToString();

        var resultError = ResultErrorKeyNotFound.Create(key, sourceDescription);

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)httpStatusCode));
        problemDetails.Detail.Should().Be($"Unable to find key : '{key}' in source : '{sourceDescription}'");
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be((int)httpStatusCode);
    }
    
    [Fact]
    public void when_NanoControllerResultError()
    {
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();
        
        var httpStatusCode = HttpStatusCode.BadRequest;
        var detailErrorMessage = Guid.NewGuid().ToString();

        var resultError = new WebApiResultError(httpStatusCode, detailErrorMessage);

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)httpStatusCode));
        problemDetails.Detail.Should().Be(detailErrorMessage);
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be((int)httpStatusCode);
    }
    
    [Fact]
    public void when_FluentValidationResultError()
    {
        var property1 = Guid.NewGuid().ToString();
        var property2 = Guid.NewGuid().ToString();
        var errorMessage1 = Guid.NewGuid().ToString();
        var errorMessage2 = Guid.NewGuid().ToString();
        
        var failedStatusCode = 503;
        var requestPath = Guid.NewGuid().ToString();

        var validationFailures = new List<ValidationFailure>
        {
            new(property1, errorMessage1),
            new(property2, errorMessage2),
        };
        var resultError = new ValidationResult(validationFailures).ToResultError();

        var problemDetails = resultError.ToProblemDetails(failedStatusCode, requestPath);

        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        problemDetails.Detail.Should().Be($"Validation failure for '{property1}' with error : '{errorMessage1}'," +
                                          $"Validation failure for '{property2}' with error : '{errorMessage2}'");
        problemDetails.Instance.Should().Be(requestPath);
        problemDetails.Status.Should().Be(failedStatusCode);
    }
}