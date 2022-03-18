using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using FluentValidation;

using Futurum.Microsoft.Extensions.DependencyInjection;
using Futurum.NanoController.Internal;
using Futurum.NanoController.Middleware;

namespace Futurum.NanoController;

[ExcludeFromCodeCoverage]
public class NanoControllerModule : IModule
{
    private readonly NanoControllerConfiguration _configuration;
    private readonly Assembly[] _assemblies;

    public NanoControllerModule(NanoControllerConfiguration configuration, params Assembly[] assemblies)
    {
        _configuration = configuration;
        _assemblies = assemblies;
    }

    public NanoControllerModule(params Assembly[] assemblies)
        : this(NanoControllerConfiguration.Default, assemblies)
    {
    }

    public void Load(IServiceCollection services)
    {
        RegisterConfiguration(services, _configuration);
        
        services.AddScoped<INanoControllerRouter, NanoControllerRouter>();


        IMetadataCache metadataCache = new MetadataCache();
        metadataCache.Register(_assemblies);

        services.AddSingleton(metadataCache);

        foreach (var metadataTypeDefinition in metadataCache.MetadataTypeDefinitions)
        {
            var handlerServiceType = metadataTypeDefinition.HandlerInterfaceType;
            var handlerImplementationType = metadataTypeDefinition.HandlerType;

            services.AddScoped(handlerServiceType, handlerImplementationType);
            
            var dispatcherServiceType = typeof(INanoControllerDispatcher<,>).MakeGenericType(metadataTypeDefinition.RequestType, metadataTypeDefinition.ResponseType);
            var dispatcherImplementationType = typeof(NanoControllerDispatcher<,>).MakeGenericType(metadataTypeDefinition.RequestType, metadataTypeDefinition.ResponseType);

            services.AddScoped(dispatcherServiceType, dispatcherImplementationType);
        }

        RegisterValidation(services, _assemblies);

        RegisterMiddleware(services, _configuration);
    }

    private static void RegisterConfiguration(IServiceCollection services, NanoControllerConfiguration configuration)
    {
        services.AddSingleton(configuration);
    }

    private static void RegisterValidation(IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton(typeof(INanoControllerRequestValidation<>), typeof(NanoControllerRequestValidation<>));

        services.Scan(scan => scan.FromAssemblies(assemblies)
                                  .AddClasses(classes => classes.Where(type => type.IsClosedTypeOf(typeof(IValidator<>))))
                                  .AsImplementedInterfaces()
                                  .WithSingletonLifetime());
    }

    private static void RegisterMiddleware(IServiceCollection services, NanoControllerConfiguration configuration)
    {
        if (configuration.EnableMiddleware)
        {
            RegisterEnabledMiddleware(services);
        }
        else
        {
            RegisterDisabledMiddleware(services);
        }
    }

    private static void RegisterDisabledMiddleware(IServiceCollection services)
    {
        services.AddSingleton(typeof(INanoControllerMiddlewareExecutor<,>), typeof(DisabledNanoControllerMiddlewareExecutor<,>));
    }
    
    private static void RegisterEnabledMiddleware(IServiceCollection services)
    {
        services.AddSingleton(typeof(INanoControllerMiddlewareExecutor<,>), typeof(NanoControllerMiddlewareExecutor<,>));
        
        services.AddSingleton(typeof(INanoControllerMiddleware<,>), typeof(NanoControllerPreProcessorMiddleware<,>));
        services.AddSingleton(typeof(INanoControllerMiddleware<,>), typeof(NanoControllerPostProcessorMiddleware<,>));
    }
}