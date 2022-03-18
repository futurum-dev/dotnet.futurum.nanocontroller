using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Futurum.NanoController.OpenApi;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
public class ConfigureOpenApiVersioningOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly IOpenApiInformation _openApiInformation;

    public ConfigureOpenApiVersioningOptions(IApiVersionDescriptionProvider provider,
                                             IOpenApiInformation openApiInformation)
    {
        _provider = provider;
        _openApiInformation = openApiInformation;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var openApiInfo = _openApiInformation.Get;

        openApiInfo.Title = $"{openApiInfo.Title}{(description.IsDeprecated ? " - DEPRECATED" : "")}";

        if (description.IsDeprecated)
        {
            openApiInfo.Description = $"{openApiInfo.Description} - This API version has been deprecated.";
        }

        return openApiInfo;
    }
}