using Futurum.NanoController.OpenApi;
using Microsoft.OpenApi.Models;

namespace Futurum.NanoController.Sample;

public class OpenApiInformation : IOpenApiInformation
{
    public OpenApiInfo Get => new()
    {
        Title = "Futurum.NanoController.Samples"
    };
}