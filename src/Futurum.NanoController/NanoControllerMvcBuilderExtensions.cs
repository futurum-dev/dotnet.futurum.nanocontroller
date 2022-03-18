using Futurum.NanoController.Internal;

namespace Futurum.NanoController;

public static class NanoControllerMvcBuilderExtensions
{
    public static IMvcBuilder AddNanoControllers(this IServiceCollection serviceCollection) =>
        serviceCollection.AddControllers(options =>
                         {
                             options.Conventions.Add(new AspNetCoreCustomRouteTokenConvention("namespace", controllerModel => controllerModel.ControllerType.Namespace?.Split('.').Last()));
                             options.Conventions.Add(new AspNetCoreCustomRouteTokenConvention("static-class", controllerModel => controllerModel.ControllerType.FullName?.Split('.').Last().Split('+').First()));
                         })
                         .ConfigureApplicationPartManager(applicationPartManager => applicationPartManager.FeatureProviders.Add(new NanoControllerFeatureProvider()));
}