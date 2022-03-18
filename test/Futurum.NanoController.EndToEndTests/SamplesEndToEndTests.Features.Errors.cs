using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Futurum.NanoController.Sample.Features.Error;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Futurum.NanoController.EndToEndTests;

[Collection("Sequential")]
public class SamplesEndToEndFeaturesErrorsTests
{
    [Fact]
    public async Task ErrorResult()
    {
        var httpClient = CreateClient();

        var commandDto = new ErrorResultScenario.CommandDto(Guid.NewGuid().ToString());
        var json = JsonSerializer.Serialize(commandDto);

        var requestPath = "/api/v1/error-result-scenario";
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestPath),
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var httpResponseMessage = await httpClient.SendAsync(request);

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

        response.Title.Should().Be(ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.BadRequest));
        response.Detail.Should().Be($"An result error has occured - {commandDto.Id}");
        response.Status.Should().Be((int)HttpStatusCode.BadRequest);
        response.Instance.Should().Be(requestPath);
    }
    
    [Fact]
    public async Task ErrorException()
    {
        var httpClient = CreateClient();

        var commandDto = new ErrorExceptionScenario.CommandDto(Guid.NewGuid().ToString());
        var json = JsonSerializer.Serialize(commandDto);

        var requestPath = "/api/v1/error-exception-scenario";
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestPath),
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var httpResponseMessage = await httpClient.SendAsync(request);

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

        response.Title.Should().Be("Internal Server Error");
        response.Detail.Should().Contain($"An exception has occured - {commandDto.Id}");
        response.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        response.Instance.Should().Be(requestPath);
    }

    private static HttpClient CreateClient() =>
        new WebApplicationFactory<Sample.Program>()
            .CreateClient();
}