using Microsoft.OpenApi.Models;

namespace Futurum.NanoController.OpenApi;

public interface IOpenApiInformation
{
    OpenApiInfo Get { get; }
}