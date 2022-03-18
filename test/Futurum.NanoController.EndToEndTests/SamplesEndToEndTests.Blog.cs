using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Futurum.NanoController.Sample.Blog;

using Microsoft.AspNetCore.Mvc.Testing;

using Xunit;

namespace Futurum.NanoController.EndToEndTests;

[Collection("Sequential")]
public class SamplesEndToEndBlogTests
{
    [Fact]
    public async Task Get()
    {
        var httpClient = CreateClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("/api/v1/blog"),
        };

        var httpResponseMessage = await httpClient.SendAsync(request);

        httpResponseMessage.EnsureSuccessStatusCode();
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<DataCollectionDto<BlogDto>>();

        response.Data.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Create()
    {
        var httpClient = CreateClient();
    
        var commandDto = new BlogCreate.CommandDto(Guid.NewGuid().ToString());
        var json = JsonSerializer.Serialize(commandDto);
    
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/v1/blog"),
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
    
        var httpResponseMessage = await httpClient.SendAsync(request);
    
        httpResponseMessage.EnsureSuccessStatusCode();
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<BlogDto>();
    
        response.Should().BeEquivalentTo(commandDto);
    }
    
    [Fact]
    public async Task Update()
    {
        var httpClient = CreateClient();
    
        var createCommandDto = new BlogCreate.CommandDto(Guid.NewGuid().ToString());
        var createJson = JsonSerializer.Serialize(createCommandDto);
    
        var createRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/v1/blog"),
            Content = new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
    
        var createHttpResponseMessage = await httpClient.SendAsync(createRequest);
    
        createHttpResponseMessage.EnsureSuccessStatusCode();
        var createResponse = await createHttpResponseMessage.Content.ReadFromJsonAsync<BlogDto>();
    
        createResponse.Should().BeEquivalentTo(createCommandDto);
    
        var updateCommandDto = new BlogUpdate.CommandDto(createResponse.Id, Guid.NewGuid().ToString());
        var updateJson = JsonSerializer.Serialize(updateCommandDto);
    
        var updateRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri("/api/v1/blog"),
            Content = new StringContent(updateJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
    
        var updateHttpResponseMessage = await httpClient.SendAsync(updateRequest);
    
        updateHttpResponseMessage.EnsureSuccessStatusCode();
        var updateResponse = await updateHttpResponseMessage.Content.ReadFromJsonAsync<BlogDto>();
    
        updateResponse.Should().BeEquivalentTo(updateCommandDto);
    }
    
    [Fact]
    public async Task Delete()
    {
        var httpClient = CreateClient();
    
        var createCommandDto = new BlogCreate.CommandDto(Guid.NewGuid().ToString());
        var createJson = JsonSerializer.Serialize(createCommandDto);
    
        var createRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/v1/blog"),
            Content = new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
    
        var createHttpResponseMessage = await httpClient.SendAsync(createRequest);
    
        createHttpResponseMessage.EnsureSuccessStatusCode();
        var createResponse = await createHttpResponseMessage.Content.ReadFromJsonAsync<BlogDto>();
    
        createResponse.Should().BeEquivalentTo(createCommandDto);
    
        var deleteCommandDto = new BlogDelete.CommandDto(createResponse.Id);
        var deleteJson = JsonSerializer.Serialize(deleteCommandDto);
    
        var deleteRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/v1/blog"),
            Content = new StringContent(deleteJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
    
        var deleteHttpResponseMessage = await httpClient.SendAsync(deleteRequest);
    
        deleteHttpResponseMessage.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Composite()
    {
        var httpClient = CreateClient();
    
        var commandDto = new BlogCreate.CommandDto(Guid.NewGuid().ToString());
        var json = JsonSerializer.Serialize(commandDto);
    
        var createRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/v1/blog"),
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
    
        var createHttpResponseMessage = await httpClient.SendAsync(createRequest);
    
        createHttpResponseMessage.EnsureSuccessStatusCode();
        var createResponse = await createHttpResponseMessage.Content.ReadFromJsonAsync<BlogDto>();
    
        createResponse.Url.Should().Be(commandDto.Url);
    
        var getRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("/api/v1/blog"),
        };
    
        var getHttpResponseMessage = await httpClient.SendAsync(getRequest);
    
        getHttpResponseMessage.EnsureSuccessStatusCode();
        var getResponse = await getHttpResponseMessage.Content.ReadFromJsonAsync<DataCollectionDto<BlogDto>>();
    
        getResponse.Count.Should().Be(1);
        getResponse.Data.Single().Url.Should().Be(commandDto.Url);
    }

    private static HttpClient CreateClient() =>
        new WebApplicationFactory<Sample.Program>()
            .CreateClient();
}