using Futurum.NanoController.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Futurum.NanoController.OpenApi;

public static class NanoControllerOpenApiExtensions
{
    /// <summary>
    /// Updates OpenApi document to support NanoController.<br/>
    /// For controllers inherited from <see cref="NanoController" /><br/>
    /// - Replaces action Tag with slugified <c>[namespace]</c><br/>
    /// </summary>
    public static void UseNanoControllers(this SwaggerGenOptions options)
    {
        options.TagActionsBy(NanoControllerNamespaceOrDefault);
        options.CustomSchemaIds(type => type.FullName); // Fixes this issue - https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1607#issuecomment-607170559
    }

    private static IList<string?> NanoControllerNamespaceOrDefault(ApiDescription apiDescription)
    {
        if (apiDescription.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
        {
            throw new InvalidOperationException($"Unable to determine tag for endpoint: {apiDescription.ActionDescriptor.DisplayName}");
        }

        if (typeof(NanoController.Base).IsAssignableFrom(actionDescriptor.ControllerTypeInfo))
        {
            var controllerNamespace = actionDescriptor.ControllerTypeInfo.Namespace?.Split('.').Last();

            if (controllerNamespace != null)
            {
                var tagName = controllerNamespace.ToSlugify();

                return new[] { tagName };
            }
        }

        return new[] { actionDescriptor.ControllerName.ToSlugify() };
    }

    public static IServiceCollection EnableApiVersioningForNanoControllers(this IServiceCollection serviceCollection, ApiVersion? defaultApiVersion)
    {
        defaultApiVersion ??= ApiVersion.Default;
        serviceCollection.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = defaultApiVersion;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        serviceCollection.AddVersionedApiExplorer(
            options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;

                options.DefaultApiVersion = defaultApiVersion;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

        return serviceCollection;
    }

    public static IServiceCollection EnableOpenApiForNanoControllers<TOpenApiInformation>(this IServiceCollection serviceCollection)
        where TOpenApiInformation : class, IOpenApiInformation
    {
        serviceCollection.AddTransient<IOpenApiInformation, TOpenApiInformation>();
        serviceCollection.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureOpenApiVersioningOptions>();
        serviceCollection.AddTransient<IOptionsSnapshot<SwaggerUIOptions>, ConfigureOpenApiUIVersioningOptions>();
        serviceCollection.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.EnableAnnotations();
            swaggerGenOptions.UseNanoControllers();
        });

        return serviceCollection;
    }

    public static WebApplication UseOpenApiUIForNanoControllers(this WebApplication application)
    {
        application.UseSwagger();
        application.UseSwaggerUI();

        return application;
    }
}