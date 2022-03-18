using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace Futurum.NanoController.OpenApi;

public class ConfigureOpenApiUIVersioningOptions : IOptionsSnapshot<SwaggerUIOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

    public ConfigureOpenApiUIVersioningOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
    }

    public SwaggerUIOptions Value
    {
        get
        {
            var options = new SwaggerUIOptions();

            // build a swagger endpoint for each discovered API version
            foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }

            return options;
        }
    }

    public SwaggerUIOptions Get(string name) => Value;
}