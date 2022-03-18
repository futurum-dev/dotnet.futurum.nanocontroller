using Futurum.NanoController.OpenApi;

using Microsoft.OpenApi.Models;

namespace Futurum.NanoController.Benchmark.NanoController;

public class OpenApiInformation : IOpenApiInformation
{
    public OpenApiInfo Get => new()
    {
        Title = "Futurum.NanoController.Benchmark.NanoController"
    };
}