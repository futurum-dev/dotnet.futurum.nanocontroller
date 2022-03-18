using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Futurum.NanoController.Sample.Features;
using Futurum.NanoController.Sample.Features.CommandWithResponse;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Futurum.NanoController.EndToEndTests;

public class SamplesEndToEndFeaturesTests
{
    [Collection("Sequential")]
    public class Query
    {
        [Fact]
        public async Task ByQuery()
        {
            var httpClient = CreateClient();

            var id = Guid.NewGuid().ToString();
            
            var httpResponseMessage = await httpClient.GetAsync($"/api/v1/query-by-query-with-response-scenario?id={id}");

            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<FeatureDto>();

            response.Name.Should().Be($"Name - {id}");
        }

        [Fact]
        public async Task ById()
        {
            var httpClient = CreateClient();

            var id = Guid.NewGuid().ToString();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"/api/v1/query-by-id-with-response-scenario/{id}")
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<FeatureDto>();

            response.Name.Should().Be($"Name - {id}");
        }

        [Fact]
        public async Task WithoutRequest()
        {
            var httpClient = CreateClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"/api/v1/query-without-request-with-response-scenario")
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<FeatureDto>();

            response.Name.Should().NotBeEmpty();
        }
    }

    [Collection("Sequential")]
    public class Command
    {
        [Fact]
        public async Task WithResponse()
        {
            var httpClient = CreateClient();

            var commandDto = new CommandWithRequestWithResponseScenario.CommandDto(Guid.NewGuid().ToString());
            var json = JsonSerializer.Serialize(commandDto);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/v1/command-with-request-with-response-scenario"),
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<FeatureDto>();

            response.Name.Should().Be($"Name - {commandDto.Id}");
        }

        [Fact]
        public async Task WithoutResponse()
        {
            var httpClient = CreateClient();

            var commandDto = new CommandWithRequestWithResponseScenario.CommandDto(Guid.NewGuid().ToString());
            var json = JsonSerializer.Serialize(commandDto);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/v1/command-with-request-without-response-scenario"),
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
        }
    }

    [Collection("Sequential")]
    public class UploadFile
    {
        [Fact]
        public async Task WithResponse()
        {
            var httpClient = CreateClient();

            await using var fileStream = File.OpenRead("./Data/hello-world.txt");

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StreamContent(fileStream), name: "formFile", fileName: "hello-world.txt");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/v1/request-upload-file-with-response-scenario"),
                Content = multipartFormDataContent
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<FeatureDto>();

            response.Name.Should().Be($"Name - 0 - hello-world.txt");
        }

        [Fact]
        public async Task WithoutResponse()
        {
            var httpClient = CreateClient();

            await using var fileStream = File.OpenRead("./Data/hello-world.txt");

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StreamContent(fileStream), name: "formFile", fileName: "hello-world.txt");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/v1/request-upload-file-without-response-scenario"),
                Content = multipartFormDataContent
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
        }
    }

    [Collection("Sequential")]
    public class UploadFiles
    {
        [Fact]
        public async Task WithResponse()
        {
            var httpClient = CreateClient();

            await using var fileStream = File.OpenRead("./Data/hello-world.txt");

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StreamContent(fileStream), name: "formFiles", fileName: "hello-world.txt");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/v1/request-upload-files-with-response-scenario"),
                Content = multipartFormDataContent
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<FeatureDto>();

            response.Name.Should().Be($"Name - 0 - hello-world.txt");
        }

        [Fact]
        public async Task WithoutResponse()
        {
            var httpClient = CreateClient();

            await using var fileStream = File.OpenRead("./Data/hello-world.txt");

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StreamContent(fileStream), name: "formFiles", fileName: "hello-world.txt");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/v1/request-upload-files-without-response-scenario"),
                Content = multipartFormDataContent
            };

            var httpResponseMessage = await httpClient.SendAsync(request);

            httpResponseMessage.EnsureSuccessStatusCode();
        }
    }

    private static HttpClient CreateClient() =>
        new WebApplicationFactory<Sample.Program>()
            .CreateClient();
}