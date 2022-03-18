using System.Net.Mime;
using System.Text;
using System.Text.Json;

using BenchmarkDotNet.Attributes;

using Futurum.NanoController.Benchmark.NanoController;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Futurum.NanoController.Benchmark.Runner;

[
    MemoryDiagnoser,
    SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 20, invocationCount: 20000)
]
public class Benchmarks
{
    private static readonly HttpClient NanoControllerClient = new WebApplicationFactory<Futurum.NanoController.Benchmark.NanoController.Program>().CreateClient();
    private static readonly HttpClient MvcControllerClient = new WebApplicationFactory<Futurum.NanoController.Benchmark.MvcController.Program>().CreateClient();

    private static readonly StringContent NanoControllerRequestPayload =
        new(JsonSerializer.Serialize(new TestNanoController.RequestDto
            {
                Body = new TestNanoController.RequestDto.BodyDto
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Age = 55,
                    PhoneNumbers = new[]
                    {
                        "1111111111",
                        "2222222222",
                        "3333333333",
                        "4444444444",
                        "5555555555"
                    }
                }
            }),
            Encoding.UTF8, MediaTypeNames.Application.Json);

    private static readonly StringContent MvcControllerRequestPayload =
        new(JsonSerializer.Serialize(new MvcController.RequestDto("FirstName", "LastName", 55, new[]
            {
                "1111111111",
                "2222222222",
                "3333333333",
                "4444444444",
                "5555555555"
            })),
            Encoding.UTF8, MediaTypeNames.Application.Json);

    [Benchmark(Baseline = true)]
    public async Task NanoController()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/benchmark/22"),
            Content = NanoControllerRequestPayload
        };
        
        var response = await NanoControllerClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    [Benchmark]
    public async Task MvcControllers()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/benchmark/22"),
            Content = MvcControllerRequestPayload
        };

        var response = await MvcControllerClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}